using OpenChat.PlaygroundApp.Abstractions;
using OpenChat.PlaygroundApp.Configurations;
using OpenChat.PlaygroundApp.Constants;

namespace OpenChat.PlaygroundApp.Options;

/// <summary>
/// This represents the argument options entity for Google Vertex AI.
/// </summary>
public class GoogleVertexAIArgumentOptions : ArgumentOptions
{
    public string? ApiKey { get; set; }
    public string? Model { get; set; }
    public string? AccessToken { get; set; }
    public string? ProjectId { get; set; }
    public string? Region { get; set; }

    protected override void ParseOptions(IConfiguration config, string[] args)
    {
        var settings = new AppSettings();
        config.Bind(settings);
        var google = settings.GoogleVertexAI;
        this.ApiKey ??= google?.ApiKey;
        this.Model ??= google?.Model;
        this.AccessToken ??= google?.AccessToken;
        this.ProjectId ??= google?.ProjectId;
        this.Region ??= google?.Region;
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case ArgumentOptionConstants.GoogleVertexAI.ApiKey:
                    if (i + 1 < args.Length)
                    {
                        this.ApiKey = args[++i];
                    }
                    break;

                case ArgumentOptionConstants.GoogleVertexAI.Model:
                    if (i + 1 < args.Length)
                    {
                        this.Model = args[++i];
                    }
                    break;
                case ArgumentOptionConstants.GoogleVertexAI.AccessToken:
                    if (i + 1 < args.Length)
                    {
                        this.AccessToken = args[++i];
                    }
                    break;
                case ArgumentOptionConstants.GoogleVertexAI.ProjectId:
                    if (i + 1 < args.Length)
                    {
                        this.ProjectId = args[++i];
                    }
                    break;
                case ArgumentOptionConstants.GoogleVertexAI.Region:
                    if (i + 1 < args.Length)
                    {
                        this.Region = args[++i];
                    }
                    break;
                default:
                    break;
            }
        }
    }
}