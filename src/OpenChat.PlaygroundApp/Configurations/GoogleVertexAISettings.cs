using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

/// <inheritdoc/>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="GoogleVertexAISettings"/> instance.
    /// </summary>
    public GoogleVertexAISettings? GoogleVertexAI { get; set; }
}

/// <summary>
/// This represents the app settings entity for Google Vertex AI.
/// </summary>
public class GoogleVertexAISettings : LanguageModelSettings
{
    public string? ApiKey { get; set; }
    public string? Model { get; set; }
    public string? AccessToken { get; set; }
    public string? ProjectId { get; set; }
    public string? Region { get; set; }
}