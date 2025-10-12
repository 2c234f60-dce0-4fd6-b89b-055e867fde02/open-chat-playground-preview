using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Components;
using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Endpoints;
using OpenChat.PlaygroundApp.OpenApi;
using OpenChat.PlaygroundApp.Services;

using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var config = builder.Configuration;
var settings = ArgumentOptions.Parse(config, args);
if (settings.Help == true)
{
    ArgumentOptions.DisplayHelp();
    return;
}

builder.Services.AddSingleton(new OpenChat.PlaygroundApp.Abstractions.ConnectorTypeInfo(settings.ConnectorType.ToString()));

builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

// OpenTelemetry
string sourceName = Guid.NewGuid().ToString();
TracerProvider tracerProvider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
    .AddSource(sourceName)
    .AddConsoleExporter()
    .Build();

// Cache
IDistributedCache cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
var chatClients = new Dictionary<ConnectorType, IChatClient>();

// ChatService
var enabledTypes = settings.EnabledConnectorTypes?.Count > 0
    ? settings.EnabledConnectorTypes
    : new List<ConnectorType> { settings.ConnectorType };
foreach (var connectorType in enabledTypes)
{
    var chatClient = await LanguageModelConnector.CreateChatClientAsync(settings, connectorType);
    builder.Services.AddChatClient(chatClient)
                    .UseDistributedCache(cache)
                    .UseFunctionInvocation()
                    .UseOpenTelemetry(
                        sourceName: sourceName,
                        configure: c => c.EnableSensitiveData = true
                    )
                    .UseLogging();

    chatClients[connectorType] = chatClient;
}
builder.Services.AddSingleton<IDictionary<ConnectorType, IChatClient>>(chatClients);

builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi("openapi", options =>
{
    options.AddDocumentTransformer<OpenApiDocumentTransformer>();
});

builder.Services.AddScoped<IChatService, ChatService>(sp =>
    new ChatService(
        sp.GetRequiredService<IDictionary<ConnectorType, IChatClient>>(),
        sp.GetRequiredService<ILogger<ChatService>>()
    )
);
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    app.UseHttpsRedirection();
}

app.UseAntiforgery();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/{documentName}.json");
}

var group = app.MapGroup("/api");
app.MapEndpoints(group);

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

await app.RunAsync();
