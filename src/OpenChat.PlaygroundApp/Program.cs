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
builder.Services.AddLogging();
builder.AddServiceDefaults();

var config = builder.Configuration;
var settings = ArgumentOptions.Parse(config, args);
if (settings.Help == true)
{
    ArgumentOptions.DisplayHelp();
    return;
}

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
var chatClientFactories = new Dictionary<ConnectorType, Func<IServiceProvider, IChatClient>>();

// ChatService
builder.Services.AddSingleton(new OpenChat.PlaygroundApp.Abstractions.ConnectorTypeInfo(settings.ConnectorType.ToString()));
var enabledTypes = settings.EnabledConnectorTypes?.Count > 0
    ? settings.EnabledConnectorTypes
    : Enum.GetValues(typeof(ConnectorType)).Cast<ConnectorType>().Where(t => t != ConnectorType.Unknown).ToList();

foreach (var connectorType in enabledTypes)
{
    try
    {
        settings.ConnectorType = connectorType;
        var chatClient = await LanguageModelConnector.CreateChatClientAsync(settings, connectorType);
        var builderChain = builder.Services.AddChatClient(chatClient)
            .UseDistributedCache(cache)
            .UseFunctionInvocation()
            .UseOpenTelemetry(
                sourceName: sourceName,
                configure: c => c.EnableSensitiveData = true
            )
            .UseLogging();
        chatClientFactories[connectorType] = sp => builderChain.Build(sp);
    }
    catch (Exception ex)
    {
        // 설정이 없는 경우: 명확한 예외를 발생시키는 Dummy Factory 등록
        chatClientFactories[connectorType] = sp => throw new InvalidOperationException($"ConnectorType '{connectorType}' is not properly configured: {ex.Message}");
    }
}
builder.Services.AddSingleton<IDictionary<ConnectorType, Func<IServiceProvider, IChatClient>>>(chatClientFactories);

builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi("openapi", options =>
{
    options.AddDocumentTransformer<OpenApiDocumentTransformer>();
});

builder.Services.AddScoped<IChatService, ChatService>(sp =>
    new ChatService(
        sp.GetRequiredService<IDictionary<ConnectorType, Func<IServiceProvider, IChatClient>>>(),
        sp,
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
