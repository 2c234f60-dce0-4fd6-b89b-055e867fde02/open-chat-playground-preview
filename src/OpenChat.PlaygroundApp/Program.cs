using Microsoft.AspNetCore.Mvc;
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

// Configure OpenTelemetry
string sourceName = Guid.NewGuid().ToString();
TracerProvider tracerProvider = OpenTelemetry.Sdk.CreateTracerProviderBuilder()
    .AddSource(sourceName)
    .AddConsoleExporter()
    .Build();

// Configure Cache
IDistributedCache cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
builder.Services.AddSingleton<IDistributedCache>(cache);

// Configure Chat Clients
foreach (ConnectorType connectorType in Enum.GetValues(typeof(ConnectorType)))
{
    // TODO: settings.ConnectorType = connectorType; 이 부분 리팩토링 필요함. 지금은 1:1, N개 한번에 할 수 있는 형태로.
    if (connectorType == ConnectorType.Unknown) continue;
    settings.ConnectorType = connectorType;

    var chatClient = await LanguageModelConnector.CreateChatClientAsync(settings);
    if (chatClient is null) continue;

    builder.Services.AddChatClient(chatClient)
                    .UseDistributedCache(cache)
                    .UseFunctionInvocation()
                    .UseOpenTelemetry(
                        sourceName: sourceName,
                        configure: c => c.EnableSensitiveData = true
                    )
                    .UseLogging();
}

builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi("openapi", options =>
{
    options.AddDocumentTransformer<OpenApiDocumentTransformer>();
});

builder.Services.AddScoped<IChatService, ChatService>();
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

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/{documentName}.json");
}

// 외부 API 엔드포인트
var group = app.MapGroup("/api");
app.MapEndpoints(group);
group.MapPost("/chat/{connectorType}", async (string connectorType, [FromBody] List<ChatMessage> messages, IChatService chatService) =>
{
    if (!Enum.TryParse<ConnectorType>(connectorType, true, out var type) || type == ConnectorType.Unknown)
    {
        return Results.BadRequest($"Invalid connectorType: {connectorType}");
    }
    var chatClient = chatService;
    if (chatClient == null)
    {
        return Results.NotFound($"ChatClient for {connectorType} not found");
    }
    if (messages == null || messages.Count < 2)
    {
        return Results.BadRequest("At least two messages (system, user) required");
    }

    var options = new ChatOptions();
    var response = new List<string>();
    await foreach (var update in chatClient.GetStreamingResponseAsync(messages, options))
    {
        response.Add(update.Text);
    }
    return Results.Ok(string.Join("", response));
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
