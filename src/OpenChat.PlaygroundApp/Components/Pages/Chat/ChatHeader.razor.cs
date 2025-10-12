using Microsoft.AspNetCore.Components;

using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Components.Pages.Chat;

public partial class ChatHeader : ComponentBase
{    
    [Parameter]
    public EventCallback OnNewChat { get; set; }

    // ConnectorType 선택 관련
    private List<OpenChat.PlaygroundApp.Connectors.ConnectorType> ConnectorTypes = new();

    public OpenChat.PlaygroundApp.Connectors.ConnectorType selectedConnectorType { get; set; }

    protected override void OnInitialized()
    {
        ConnectorTypes = Enum.GetValues(typeof(OpenChat.PlaygroundApp.Connectors.ConnectorType))
            .Cast<OpenChat.PlaygroundApp.Connectors.ConnectorType>()
            .Where(t => t != OpenChat.PlaygroundApp.Connectors.ConnectorType.Unknown)
            .ToList();

        selectedConnectorType = ConnectorTypes.FirstOrDefault();
    }
}
