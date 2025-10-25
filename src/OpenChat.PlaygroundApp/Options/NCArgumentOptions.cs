using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Options;

public class NCArgumentOptions : ArgumentOptions
{
    public string? BaseUrl { get; set; }
    public string? Model { get; set; }

    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);
        var nc = settings.NC;
        this.BaseUrl ??= nc?.BaseUrl;
        this.Model ??= nc?.Model;
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--base-url":
                    if (i + 1 < args.Length) this.BaseUrl = args[++i];
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
