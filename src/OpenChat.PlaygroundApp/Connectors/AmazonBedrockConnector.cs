using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Microsoft.Extensions.AI;
using Amazon.BedrockRuntime;
using Amazon;

namespace OpenChat.PlaygroundApp.Connectors;

public class AmazonBedrockConnector(AppSettings settings) : LanguageModelConnector(settings.AmazonBedrock)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));

    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not AmazonBedrockSettings settings)
        {
            throw new InvalidOperationException("Missing configuration: AmazonBedrock.");
        }
        if (string.IsNullOrWhiteSpace(settings.Region?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: AmazonBedrock:Region.");
        }
        if (string.IsNullOrWhiteSpace(settings.ModelId?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: AmazonBedrock:ModelId.");
        }
        if (string.IsNullOrWhiteSpace(settings.AccessKeyId?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: AmazonBedrock:AccessKeyId.");
        }
        if (string.IsNullOrWhiteSpace(settings.SecretAccessKey?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: AmazonBedrock:SecretAccessKey.");
        }
        return true;
    }

    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as AmazonBedrockSettings;

        var client = new AmazonBedrockRuntimeClient(
            awsAccessKeyId: settings!.AccessKeyId!,
            awsSecretAccessKey: settings!.SecretAccessKey!,
            region: RegionEndpoint.GetBySystemName(settings!.Region!)
        );
        var chatClient = client.AsIChatClient(
            settings!.ModelId!
        );

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {settings.ModelId}");

        return await Task.FromResult(chatClient).ConfigureAwait(false);
    }
}
