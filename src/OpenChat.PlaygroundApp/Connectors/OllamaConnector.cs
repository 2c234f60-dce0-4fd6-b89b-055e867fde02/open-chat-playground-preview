using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Microsoft.Extensions.AI;
using OllamaSharp;

namespace OpenChat.PlaygroundApp.Connectors;

public class OllamaConnector(AppSettings settings) : LanguageModelConnector(settings.Ollama)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not OllamaSettings settings)
        {
            throw new InvalidOperationException("Missing configuration: Ollama.");
        }
        if (string.IsNullOrWhiteSpace(settings.BaseUrl))
        {
            throw new InvalidOperationException("Missing configuration: Ollama:BaseUrl.");
        }
        if (string.IsNullOrWhiteSpace(settings.Model))
        {
            throw new InvalidOperationException("Missing configuration: Ollama:Model.");
        }
        return true;
    }

    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as OllamaSettings;

        var model = settings!.Model!;
        var client = new OllamaApiClient(new Uri(settings.BaseUrl!))
        {
            SelectedModel = model
        };
        var chatClient = client as IChatClient;

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {settings.Model}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}
