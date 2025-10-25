using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Options;

public class NaverArgumentOptions : ArgumentOptions
{
    public string? BaseUrl { get; set; }
    public string? ApiKey { get; set; }
    public string? Model { get; set; }

    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);
        var naver = settings.Naver;
        this.BaseUrl ??= naver?.BaseUrl;
        this.ApiKey ??= naver?.ApiKey;
        this.Model ??= naver?.Model;
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--base-url":
                    if (i + 1 < args.Length) this.BaseUrl = args[++i];
                    break;
                case "--api-key":
                    if (i + 1 < args.Length) this.ApiKey = args[++i];
                    break;
                case "--model":
                    if (i + 1 < args.Length) this.Model = args[++i];
                    break;
                default:
                    break;
            }
        }
    }
}
