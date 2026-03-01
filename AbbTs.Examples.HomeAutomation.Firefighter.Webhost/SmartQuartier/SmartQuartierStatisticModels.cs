using System.Text.Json.Serialization;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier;

public sealed record SmartQuartierStatisticResponse(
    [property: JsonPropertyName("measurements")] SmartQuartierMeasurementStatistic Measurements,
    [property: JsonPropertyName("events")] SmartQuartierEventStatistic Events);

public sealed record SmartQuartierMeasurementStatistic(
    [property: JsonPropertyName("brightness")] SmartQuartierNumericStatistic Brightness,
    [property: JsonPropertyName("temperature")] SmartQuartierNumericStatistic Temperature,
    [property: JsonPropertyName("humidity")] SmartQuartierNumericStatistic Humidity,
    [property: JsonPropertyName("gas")] SmartQuartierNumericStatistic Gas);

public sealed record SmartQuartierNumericStatistic(
    [property: JsonPropertyName("min")] SmartQuartierStatisticSample Min,
    [property: JsonPropertyName("max")] SmartQuartierStatisticSample Max,
    [property: JsonPropertyName("average")] double Average);

public sealed record SmartQuartierStatisticSample(
    [property: JsonPropertyName("timeStamp")] DateTime TimeStamp,
    [property: JsonPropertyName("buildingID")] string BuildingId,
    [property: JsonPropertyName("value")] double Value);

public sealed record SmartQuartierEventStatistic(
    [property: JsonPropertyName("gas")] int Gas,
    [property: JsonPropertyName("fire")] int Fire,
    [property: JsonPropertyName("motion")] int Motion,
    [property: JsonPropertyName("sound")] int Sound,
    [property: JsonPropertyName("rfid")] int Rfid);