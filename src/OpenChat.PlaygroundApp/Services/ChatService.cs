using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Services;

/// <summary>
/// This provides interfaces to the chat service.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Sends chat messages and streams the response.
    /// </summary>
    /// <param name="messages">The sequence of <see cref="ChatMessage"/> to send.</param>
    /// <param name="options">The <see cref="ChatOptions"/> with which to configure the request.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The <see cref="ChatResponseUpdate"/> generated.</returns>
    IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ConnectorType connectorType,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// This represents the service entity for chat operations.
/// </summary>
public class ChatService : IChatService
{
    private readonly IDictionary<ConnectorType, IChatClient> _chatClients;
    private readonly ILogger<ChatService> _logger;

    public ChatService(IDictionary<ConnectorType, IChatClient> chatClients, ILogger<ChatService> logger)
    {
        _chatClients = chatClients ?? throw new ArgumentNullException(nameof(chatClients));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> messages,
        ConnectorType connectorType,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var chats = messages.ToList();
        if (chats.Count < 2)
        {
            throw new ArgumentException("At least two messages are required", nameof(messages));
        }

        if (chats.First().Role != ChatRole.System)
        {
            throw new ArgumentException("The first message must be a system message", nameof(messages));
        }

        if (chats.ElementAt(1).Role != ChatRole.User)
        {
            throw new ArgumentException("The second message must be a user message", nameof(messages));
        }

        _logger.LogInformation("Requesting chat response with {MessageCount} messages for {ConnectorType}", chats.Count, connectorType);
        if (!_chatClients.TryGetValue(connectorType, out var chatClient))
        {
            throw new InvalidOperationException($"ChatClient for ConnectorType '{connectorType}' is not registered.");
        }
        return chatClient.GetStreamingResponseAsync(chats, options, cancellationToken);
    }
}