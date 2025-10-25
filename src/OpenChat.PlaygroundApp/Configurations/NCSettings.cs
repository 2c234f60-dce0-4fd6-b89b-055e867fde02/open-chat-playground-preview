using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

public partial class AppSettings
{
    public NCSettings? NC { get; set; }
}

public class NCSettings : LanguageModelSettings
{
    public string? BaseUrl { get; set; }
    public string? Model { get; set; }
}
