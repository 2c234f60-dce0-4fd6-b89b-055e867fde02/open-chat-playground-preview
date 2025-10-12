using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Microsoft.Extensions.AI;
using System.ClientModel;
using OpenAI;

namespace OpenChat.PlaygroundApp.Connectors;

public class UpstageConnector(AppSettings settings) : LanguageModelConnector(settings.Upstage)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not UpstageSettings settings)
        {
            throw new InvalidOperationException("Missing configuration: Upstage.");
        }
        if (string.IsNullOrWhiteSpace(settings.BaseUrl))
        {
            throw new InvalidOperationException("Missing configuration: Upstage:BaseUrl.");
        }
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
        {
            throw new InvalidOperationException("Missing configuration: Upstage:ApiKey.");
        }
        if (string.IsNullOrWhiteSpace(settings.Model))
        {
            throw new InvalidOperationException("Missing configuration: Upstage:Model.");
        }
        return true;
    }

    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as UpstageSettings;

        var credential = new ApiKeyCredential(settings!.ApiKey!);
        var options = new OpenAIClientOptions()
        {
            Endpoint = new Uri(settings.BaseUrl!)
        };

        var client = new OpenAIClient(credential, options);
        var chatClient = client.GetChatClient(settings.Model)
                               .AsIChatClient();

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {settings.Model}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}
