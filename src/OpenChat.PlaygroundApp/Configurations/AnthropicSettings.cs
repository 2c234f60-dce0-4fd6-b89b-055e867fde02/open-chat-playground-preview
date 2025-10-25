using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

/// <inheritdoc/>
public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the <see cref="AnthropicSettings"/> instance.
    /// </summary>
    public AnthropicSettings? Anthropic { get; set; }
}

/// <summary>
/// This represents the app settings entity for Anthropic Claude.
/// </summary>
public class AnthropicSettings : LanguageModelSettings
{
    public string? ApiKey { get; set; }
    public string? Model { get; set; }
}