using OpenChat.PlaygroundApp.Abstractions;

namespace OpenChat.PlaygroundApp.Configurations;

public partial class AppSettings
{
    public NaverSettings? Naver { get; set; }
}

public class NaverSettings : LanguageModelSettings
{
    public string? BaseUrl { get; set; }
    public string? ApiKey { get; set; }
    public string? Model { get; set; }
}
