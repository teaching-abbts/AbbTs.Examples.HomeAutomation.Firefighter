using System.Text.Json.Serialization;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;

public sealed record SmartQuartierHistoryResponse(
    [property: JsonPropertyName("measurements")] IReadOnlyList<SmartQuartierMeasurement> Measurements,
    [property: JsonPropertyName("events")] IReadOnlyList<SmartQuartierEvent> Events);

public sealed record SmartQuartierMeasurement(
    [property: JsonPropertyName("timeStamp")] DateTime TimeStamp,
    [property: JsonPropertyName("buildingID")] string BuildingId,
    [property: JsonPropertyName("brightness")] int Brightness,
    [property: JsonPropertyName("temperature")] int Temperature,
    [property: JsonPropertyName("humidity")] int Humidity,
    [property: JsonPropertyName("gas")] int Gas);

public sealed record SmartQuartierEvent(
    [property: JsonPropertyName("timeStamp")] DateTime TimeStamp,
    [property: JsonPropertyName("buildingID")] string BuildingId,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("data")] string Data);