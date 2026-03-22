using System.Net.WebSockets;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

public interface ISmartHomeGateway
{
    bool IsSmartHomeConnected { get; }

    Task HandleSmartHomeSessionAsync(WebSocket webSocket, CancellationToken cancellationToken);

    Task HandleDashboardSessionAsync(WebSocket webSocket, CancellationToken cancellationToken);
}
