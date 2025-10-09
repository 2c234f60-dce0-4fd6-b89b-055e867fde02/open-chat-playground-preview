using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Constants;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Foundry Local.
/// </summary>
public class FoundryLocalArgumentOptions : ArgumentOptions
{
    public string? Alias { get; set; }

    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);
        var foundry = settings.FoundryLocal;
        this.Alias ??= foundry?.Alias;
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case ArgumentOptionConstants.FoundryLocal.Alias:
                    if (i + 1 < args.Length)
                    {
                        this.Alias = args[++i];
                    }
                    break;
                default:
                    break;
            }
        }
    }
}