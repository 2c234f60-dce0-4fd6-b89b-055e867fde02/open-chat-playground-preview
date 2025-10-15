using Microsoft.AspNetCore.Components;

using OpenChat.PlaygroundApp.Connectors;
using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Components.Pages.Chat;

public partial class ChatHeader : ComponentBase
{    
    [Parameter]
    public EventCallback OnNewChat { get; set; }

    [Parameter]
    public OpenChat.PlaygroundApp.Connectors.ConnectorType SelectedConnectorType { get; set; }

    [Parameter]
    public EventCallback<OpenChat.PlaygroundApp.Connectors.ConnectorType> OnConnectorTypeChanged { get; set; }

    [Inject]
    public required AppSettings Settings { get; set; }
    
    // ConnectorType 선택 관련
    private List<OpenChat.PlaygroundApp.Connectors.ConnectorType> ConnectorTypes = new();

    protected override void OnInitialized()
    {
        ConnectorTypes = Enum.GetValues(typeof(OpenChat.PlaygroundApp.Connectors.ConnectorType))
            .Cast<OpenChat.PlaygroundApp.Connectors.ConnectorType>()
            .Where(t => t != OpenChat.PlaygroundApp.Connectors.ConnectorType.Unknown)
            .ToList();
    }

    private async Task OnConnectorTypeChangedInternal(ChangeEventArgs e)
    {
        if (Enum.TryParse<OpenChat.PlaygroundApp.Connectors.ConnectorType>(e.Value?.ToString(), out var newType))
        {
            await OnConnectorTypeChanged.InvokeAsync(newType);
            Console.WriteLine($"Connector type changed to: {newType}");
        }
    }
}
