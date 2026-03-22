using System.Text.Json;
using System.Text.Json.Serialization;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;

public sealed record SmartHomeGatewayEnvelope(
    [property: JsonPropertyName("messageType")] string MessageType,
    [property: JsonPropertyName("payload")] JsonElement? Payload,
    [property: JsonPropertyName("receivedAtUtc")] DateTime ReceivedAtUtc);

public sealed record SmartHomeDashboardCommand(
    [property: JsonPropertyName("messageType")] string MessageType,
    [property: JsonPropertyName("payload")] JsonElement? Payload);

public sealed record SmartHomeCommand(
    [property: JsonPropertyName("device")] string Device,
    [property: JsonPropertyName("command")] string Command,
    [property: JsonPropertyName("value")] string Value);
