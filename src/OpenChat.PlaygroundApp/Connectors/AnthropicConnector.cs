using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

using Microsoft.Extensions.AI;
using Anthropic.SDK;

namespace OpenChat.PlaygroundApp.Connectors;

public class AnthropicConnector(AppSettings settings) : LanguageModelConnector(settings.Anthropic)
{
    private readonly AppSettings _appSettings = settings ?? throw new ArgumentNullException(nameof(settings));
    public override bool EnsureLanguageModelSettingsValid()
    {
        if (this.Settings is not AnthropicSettings settings)
        {
            throw new InvalidOperationException("Missing configuration: Anthropic.");
        }
        if (string.IsNullOrWhiteSpace(settings.ApiKey?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: Anthropic:ApiKey.");
        }
        if (string.IsNullOrWhiteSpace(settings.Model?.Trim()))
        {
            throw new InvalidOperationException("Missing configuration: Anthropic:Model.");
        }
        return true;
    }

    public override async Task<IChatClient> GetChatClientAsync()
    {
        var settings = this.Settings as AnthropicSettings;

        IChatClient chatClient = new ChatClientBuilder(
            new Anthropic.SDK.AnthropicClient(
                        apiKeys: new APIAuthentication(settings!.ApiKey!)
                    ).Messages
        )
        .UseFunctionInvocation()
        .Build();

        // tae0y, 이렇게 하지 않으면 Chat 화면단에서 모델ID를 options로 지정해주어야함. 최선은 아니지만, 이외에 방법은 모르겠음.
        WrappedAnthropicChatClient wrappedChatClient = new WrappedAnthropicChatClient(chatClient, settings!.Model!);

        Console.WriteLine($"The {this._appSettings.ConnectorType} connector created with model: {settings.Model}");

        return await Task.FromResult(wrappedChatClient).ConfigureAwait(false);
    }

    internal class WrappedAnthropicChatClient : IChatClient
    {
        private readonly IChatClient _inner;
        private readonly string _model;

        public WrappedAnthropicChatClient(IChatClient inner, string model)
        {
            _inner = inner;
            _model = model;
        }

        public void Dispose()
        {
            if (_inner is IDisposable d)
                d.Dispose();
        }

        public async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            options ??= new ChatOptions();
            options.ModelId = _model;
            if (options.MaxOutputTokens is null or 0)
                options.MaxOutputTokens = 1024;
            return await _inner.GetResponseAsync(messages, options, cancellationToken).ConfigureAwait(false);
        }

        public object? GetService(Type serviceType, object? serviceKey = null)
        {
            return (_inner as IServiceProvider)?.GetService(serviceType) ?? null;
        }

        public IAsyncEnumerable<ChatResponse> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            options ??= new ChatOptions();
            options.ModelId = _model;
            if (options.MaxOutputTokens is null or 0)
                options.MaxOutputTokens = 1024;
            return (IAsyncEnumerable<ChatResponse>)_inner.GetStreamingResponseAsync(messages, options, cancellationToken);
        }

        IAsyncEnumerable<ChatResponseUpdate> IChatClient.GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options, CancellationToken cancellationToken)
        {
            options ??= new ChatOptions();
            options.ModelId = _model;
            if (options.MaxOutputTokens is null or 0)
                options.MaxOutputTokens = 1024;
            return _inner.GetStreamingResponseAsync(messages, options, cancellationToken);
        }
    }
}
