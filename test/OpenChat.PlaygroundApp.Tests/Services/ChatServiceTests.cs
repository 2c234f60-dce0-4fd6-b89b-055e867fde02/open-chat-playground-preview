using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using OpenChat.PlaygroundApp.Services;

namespace OpenChat.PlaygroundApp.Tests.Services;

public class ChatServiceTests
{
    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_IChatClient_When_ChatService_Instantiated_Then_It_Should_Throw()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ChatService>>();

        // Act
        var dict = (IDictionary<OpenChat.PlaygroundApp.Connectors.ConnectorType, Func<IServiceProvider, IChatClient>>)null!;
        var sp = Substitute.For<IServiceProvider>();
        Action action = () => new ChatService(dict, sp, logger);

        // Assert
        action.ShouldThrow<ArgumentNullException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Null_Logger_When_ChatService_Instantiated_Then_It_Should_Throw()
    {
        // Arrange
        var dict = new Dictionary<OpenChat.PlaygroundApp.Connectors.ConnectorType, Func<IServiceProvider, IChatClient>>();
        var sp = Substitute.For<IServiceProvider>();

        // Act
        Action action = () => new ChatService(dict, sp, default(ILogger<ChatService>)!);

        // Assert
        action.ShouldThrow<ArgumentNullException>();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Both_Dependencies_When_ChatService_Instantiated_Then_It_Should_Create()
    {
        // Arrange
        var client = Substitute.For<IChatClient>();
        var logger = Substitute.For<ILogger<ChatService>>();
        var sp = Substitute.For<IServiceProvider>();
        var dict = new Dictionary<OpenChat.PlaygroundApp.Connectors.ConnectorType, Func<IServiceProvider, IChatClient>>
        {
            [OpenChat.PlaygroundApp.Connectors.ConnectorType.GitHubModels] = _ => client
        };

        // Act
        var result = new ChatService(dict, sp, logger);

        // Assert
        result.ShouldNotBeNull();
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Less_Than_Two_Messages_When_GetStreamingResponseAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var chatClient = Substitute.For<IChatClient>();
        var logger = Substitute.For<ILogger<ChatService>>();
        var sp = Substitute.For<IServiceProvider>();
        var dict = new Dictionary<OpenChat.PlaygroundApp.Connectors.ConnectorType, Func<IServiceProvider, IChatClient>>
        {
            [OpenChat.PlaygroundApp.Connectors.ConnectorType.GitHubModels] = _ => chatClient
        };
        var chatService = new ChatService(dict, sp, logger);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, "Hello")
        };

        // Act
        Action action = () => chatService.GetStreamingResponseAsync(messages, OpenChat.PlaygroundApp.Connectors.ConnectorType.GitHubModels);

        // Assert
        action.ShouldThrow<ArgumentException>()
              .Message.ShouldContain("At least two messages are required");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_First_Message_Is_Not_System_When_GetStreamingResponseAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var chatClient = Substitute.For<IChatClient>();
        var logger = Substitute.For<ILogger<ChatService>>();
        var sp = Substitute.For<IServiceProvider>();
        var dict = new Dictionary<OpenChat.PlaygroundApp.Connectors.ConnectorType, Func<IServiceProvider, IChatClient>>
        {
            [OpenChat.PlaygroundApp.Connectors.ConnectorType.GitHubModels] = _ => chatClient
        };
        var chatService = new ChatService(dict, sp, logger);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, "Hello"),
            new(ChatRole.User, "How are you?")
        };

        // Act
    Action action = () => chatService.GetStreamingResponseAsync(messages, OpenChat.PlaygroundApp.Connectors.ConnectorType.GitHubModels);

        // Assert
        action.ShouldThrow<ArgumentException>()
              .Message.ShouldContain("The first message must be a system message");
    }

    [Trait("Category", "UnitTest")]
    [Fact]
    public void Given_Second_Message_Is_Not_User_When_GetStreamingResponseAsync_Invoked_Then_It_Should_Throw()
    {
        // Arrange
        var chatClient = Substitute.For<IChatClient>();
        var logger = Substitute.For<ILogger<ChatService>>();
        var sp = Substitute.For<IServiceProvider>();
        var dict = new Dictionary<OpenChat.PlaygroundApp.Connectors.ConnectorType, Func<IServiceProvider, IChatClient>>
        {
            [OpenChat.PlaygroundApp.Connectors.ConnectorType.GitHubModels] = _ => chatClient
        };
        var chatService = new ChatService(dict, sp, logger);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a helpful assistant."),
            new(ChatRole.Assistant, "Why is the sky blue?")
        };

        // Act
    Action action = () => chatService.GetStreamingResponseAsync(messages, OpenChat.PlaygroundApp.Connectors.ConnectorType.GitHubModels);

        // Assert
        action.ShouldThrow<ArgumentException>()
              .Message.ShouldContain("The second message must be a user message");
    }

    [Trait("Category", "UnitTest")]
    [Theory]
    [InlineData("This ")]
    [InlineData("This ", "is ")]
    [InlineData("This ", "is ", "a ")]
    [InlineData("This ", "is ", "a ", "test.")]
    public async Task Given_Valid_Messages_When_GetStreamingResponseAsync_Invoked_Then_It_Should_Call_ChatClient(params string[] responseMessages)
    {
        // Arrange
        IEnumerable<ChatResponseUpdate> responses = responseMessages.Select(m => new ChatResponseUpdate(ChatRole.Assistant, m));

        var chatClient = Substitute.For<IChatClient>();
        chatClient.GetStreamingResponseAsync(Arg.Any<IEnumerable<ChatMessage>>(), Arg.Any<ChatOptions?>(), Arg.Any<CancellationToken>())
                  .Returns(responses.ToAsyncEnumerable());

        var logger = Substitute.For<ILogger<ChatService>>();
        var sp = Substitute.For<IServiceProvider>();
        var dict = new Dictionary<OpenChat.PlaygroundApp.Connectors.ConnectorType, Func<IServiceProvider, IChatClient>>
        {
            [OpenChat.PlaygroundApp.Connectors.ConnectorType.GitHubModels] = _ => chatClient
        };
        var chatService = new ChatService(dict, sp, logger);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, "You are a helpful assistant."),
            new(ChatRole.User, "Why is the sky blue?")
        };

        // Act
        var result = chatService.GetStreamingResponseAsync(messages, OpenChat.PlaygroundApp.Connectors.ConnectorType.GitHubModels);
        var count = await result.CountAsync();

        // Assert
        result.ShouldNotBeNull();
        count.ShouldBe(responseMessages.Length);
    }
}