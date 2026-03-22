using System.Net.WebSockets;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors.Messages;

public sealed record ConnectSmartHomeSession(SmartHomeRegistration Registration, WebSocket WebSocket);

public sealed record RecordSmartHomeEnvelope(string SmartHomeId, SmartHomeGatewayEnvelope Envelope);

public sealed record DisconnectSmartHomeSession(string SmartHomeId, WebSocket WebSocket);

public sealed record SendSmartHomeDashboardCommand(string SmartHomeId, SmartHomeDashboardCommand Command);

public sealed record GetSmartHomes;

public sealed record GetSmartHomeById(string SmartHomeId);

public sealed record AttachDeviceSocket(WebSocket WebSocket);

public sealed record MarkDeviceDisconnected(WebSocket WebSocket);

public sealed record GetSmartHomeSummary;

public sealed record GetSmartHomeDetails;

public sealed record GetSmartQuartierHistory(int? RequestedEventLimit = null);

public sealed record GetSmartQuartierStatistic;

public sealed class ScheduleDashboardHistoryBroadcast
{
    public static readonly ScheduleDashboardHistoryBroadcast Instance = new();

    private ScheduleDashboardHistoryBroadcast()
    {
    }
}