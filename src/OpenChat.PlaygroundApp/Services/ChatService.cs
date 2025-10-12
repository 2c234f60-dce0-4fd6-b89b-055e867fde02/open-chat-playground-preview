using Microsoft.Extensions.AI;

using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Services;

public interface IChatService
{
    /// <summary>
    /// Sends chat messages and streams the response.
    /// </summary>
    /// <param name="connectorType">사용할 ConnectorType</param>
    /// <param name="messages">The sequence of <see cref="ChatMessage"/> to send.</param>
    /// <param name="options">The <see cref="ChatOptions"/> with which to configure the request.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The <see cref="ChatResponseUpdate"/> generated.</returns>
    IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        ConnectorType connectorType,
        IEnumerable<ChatMessage> messages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// This represents the service entity for chat operations.
/// </summary>
/// <param name="chatClient">The <see cref="IChatClient"/>.</param>
/// <param name="logger">The <see cref="ILogger{ChatService}"/>.</param>
public class ChatService(Dictionary<ConnectorType, IChatClient> chatClients, ILogger<ChatService> logger) : IChatService
{
    private readonly Dictionary<ConnectorType, IChatClient> _chatClients = chatClients ?? throw new ArgumentNullException(nameof(chatClients));
    private readonly ILogger<ChatService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        ConnectorType connectorType,
        IEnumerable<ChatMessage> messages,
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

        this._logger.LogInformation("Requesting chat response with {MessageCount} messages for {ConnectorType}", chats.Count, connectorType);

        if (!_chatClients.TryGetValue(connectorType, out var chatClient))
        {
            throw new InvalidOperationException($"ChatClient for {connectorType} not found");
        }

        return chatClient.GetStreamingResponseAsync(chats, options, cancellationToken);
    }
}