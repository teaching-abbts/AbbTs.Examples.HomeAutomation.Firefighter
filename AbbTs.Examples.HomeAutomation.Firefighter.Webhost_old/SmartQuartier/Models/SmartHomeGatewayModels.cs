using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;

public sealed record SmartHomeGatewayEnvelope(
  [property: JsonPropertyName("messageType")] string MessageType,
  [property: JsonPropertyName("payload")] JsonElement? Payload,
  [property: JsonPropertyName("receivedAtUtc")] DateTime ReceivedAtUtc
);

public sealed record SmartHomeDashboardCommand(
  [property: JsonPropertyName("messageType")] string MessageType,
  [property: JsonPropertyName("payload")] JsonElement? Payload
);

public sealed record SmartHomeCommand(
  [property: JsonPropertyName("device")] string Device,
  [property: JsonPropertyName("command")] string Command,
  [property: JsonPropertyName("value")] string Value
);

public sealed record SmartHomeRegistration(
  [property: JsonPropertyName("buildingID")] string BuildingId,
  [property: JsonPropertyName("xCoordinate")] int XCoordinate,
  [property: JsonPropertyName("yCoordinate")] int YCoordinate,
  [property: JsonPropertyName("owner")] string Owner
);

public sealed record SmartHomeSummary(
  [property: JsonPropertyName("id")] string Id,
  [property: JsonPropertyName("owner")] string Owner,
  [property: JsonPropertyName("xCoordinate")] int XCoordinate,
  [property: JsonPropertyName("yCoordinate")] int YCoordinate,
  [property: JsonPropertyName("isConnected")] bool IsConnected,
  [property: JsonPropertyName("connectedAtUtc")] DateTime? ConnectedAtUtc,
  [property: JsonPropertyName("lastSeenUtc")] DateTime? LastSeenUtc,
  [property: JsonPropertyName("recentMessageCount")] int RecentMessageCount
);

public sealed record SmartHomeDetails(
  [property: JsonPropertyName("id")] string Id,
  [property: JsonPropertyName("owner")] string Owner,
  [property: JsonPropertyName("xCoordinate")] int XCoordinate,
  [property: JsonPropertyName("yCoordinate")] int YCoordinate,
  [property: JsonPropertyName("isConnected")] bool IsConnected,
  [property: JsonPropertyName("connectedAtUtc")] DateTime? ConnectedAtUtc,
  [property: JsonPropertyName("lastSeenUtc")] DateTime? LastSeenUtc,
  [property: JsonPropertyName("recentEnvelopes")]
    IReadOnlyList<SmartHomeGatewayEnvelope> RecentEnvelopes
);

public sealed record SmartHomeCommandResult(
  [property: JsonPropertyName("accepted")] bool Accepted,
  [property: JsonPropertyName("message")] string? Message
);
