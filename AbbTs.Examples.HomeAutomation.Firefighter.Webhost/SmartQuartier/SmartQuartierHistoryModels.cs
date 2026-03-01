using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier;

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

internal sealed class SmartQuartierTimestampJsonConverter : JsonConverter<DateTime>
{
    private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new JsonException("Expected non-empty timestamp value.");
        }

        if (DateTime.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            return parsed;
        }

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
        {
            return parsed;
        }

        throw new JsonException($"Invalid timestamp value '{value}'.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
}