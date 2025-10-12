using OpenChat.PlaygroundApp.Connectors;

using System.Text.Json;

namespace OpenChat.PlaygroundApp.Configurations;


public partial class AppSettings
{
    /// <summary>
    /// Gets or sets the connector type to use.
    /// </summary>
    public ConnectorType ConnectorType { get; set; }

    /// <summary>
    /// Gets or sets the value indicating whether to display help information or not.
    /// </summary>
    public bool Help { get; set; }

    /// <summary>
    /// Deep clone AppSettings (for per-connector isolation)
    /// </summary>
    public AppSettings DeepClone()
    {
        var json = JsonSerializer.Serialize(this);
        return JsonSerializer.Deserialize<AppSettings>(json)!;
    }
}
