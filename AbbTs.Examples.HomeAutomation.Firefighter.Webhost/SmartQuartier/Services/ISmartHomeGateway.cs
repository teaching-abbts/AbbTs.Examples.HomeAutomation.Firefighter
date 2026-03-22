using System.Net.WebSockets;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

public interface ISmartHomeGateway
{
    Task HandleSmartHomeSessionAsync(WebSocket webSocket, CancellationToken cancellationToken);

    Task<IReadOnlyList<SmartHomeSummary>> GetSmartHomesAsync(CancellationToken cancellationToken);

    Task<SmartHomeDetails?> GetSmartHomeAsync(string smartHomeId, CancellationToken cancellationToken);

    Task SendDashboardCommandAsync(string smartHomeId, SmartHomeDashboardCommand command, CancellationToken cancellationToken);
}
