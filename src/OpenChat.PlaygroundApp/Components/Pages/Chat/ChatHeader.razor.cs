using Microsoft.AspNetCore.Components;

using OpenChat.PlaygroundApp.Connectors;

namespace OpenChat.PlaygroundApp.Components.Pages.Chat;

public partial class ChatHeader : ComponentBase
{    
    [Parameter]
    public EventCallback OnNewChat { get; set; }

    // ConnectorType 선택 관련
    private List<ConnectorType> ConnectorTypes = new();
    private ConnectorType selectedConnectorType;

    protected override void OnInitialized()
    {
        ConnectorTypes = Enum.GetValues(typeof(ConnectorType))
            .Cast<ConnectorType>()
            .Where(t => t != ConnectorType.Unknown)
            .ToList();

        selectedConnectorType = ConnectorTypes.FirstOrDefault();
    }
}
