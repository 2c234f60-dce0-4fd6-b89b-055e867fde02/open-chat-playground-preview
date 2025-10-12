using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Microsoft.Extensions.AI;
using System.ClientModel;
using OpenAI;

namespace OpenChat.PlaygroundApp.Connectors;

public class DockerModelRunnerConnector(AppSettings settings) : LanguageModelConnector(settings.DockerModelRunner)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not DockerModelRunnerSettings settings)
        {
            throw new InvalidOperationException("Missing configuration: DockerModelRunner.");
        }
        if (string.IsNullOrWhiteSpace(settings.BaseUrl?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: DockerModelRunner:BaseUrl.");
        }
        if (string.IsNullOrWhiteSpace(settings.Model?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: DockerModelRunner:Model.");
        }
        return true;
    }

    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as DockerModelRunnerSettings;

        var model = settings!.Model!;
        var credential = new ApiKeyCredential(settings.BaseUrl!);
        var options = new OpenAIClientOptions()
        {
            Endpoint = new Uri(settings.BaseUrl!)
        };
        var client = new OpenAIClient(credential, options);
        var chatClient = client.GetChatClient(model)
                               .AsIChatClient();

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {settings.Model}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}
