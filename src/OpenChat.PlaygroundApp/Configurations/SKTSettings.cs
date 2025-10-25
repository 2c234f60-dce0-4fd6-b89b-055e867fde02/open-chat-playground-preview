using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

public partial class AppSettings
{
    public SKTSettings? SKT { get; set; }
}

public class SKTSettings : LanguageModelSettings
{
    public string? BaseUrl { get; set; }
    public string? Model { get; set; }
}
