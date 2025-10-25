using OpenChat.PlaygroundApp.Configurations;

namespace OpenChat.PlaygroundApp.Abstractions;

public class ConnectorTypeInfo
{
    public string Name { get; }

    public ConnectorTypeInfo(string name)
    {
        Name = name;
    }
}