using System.Text.Json.Serialization;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Models;

public sealed record HouseSensors(
  [property: JsonPropertyName("isLightOn")] bool IsLightOn,
  [property: JsonPropertyName("isDoorLocked")] bool IsDoorLocked,
  [property: JsonPropertyName("isHeatingOn")] bool IsHeatingOn,
  [property: JsonPropertyName("isAlarmOn")] bool IsAlarmOn,
  [property: JsonPropertyName("lightIntensity")] int LightIntensity,
  [property: JsonPropertyName("heatingIntensity")] int HeatingIntensity,
  [property: JsonPropertyName("displayValue")] string DisplayValue
);

public sealed record HouseCoordinates(
  [property: JsonPropertyName("x")] int X,
  [property: JsonPropertyName("y")] int Y
);

public sealed record HouseState(
  [property: JsonPropertyName("dangerKind")] string DangerKind,
  [property: JsonPropertyName("endangerment")] string Endangerment
);

public sealed record HouseSnapshot(
  [property: JsonPropertyName("buildingId")] string BuildingId,
  [property: JsonPropertyName("owner")] string Owner,
  [property: JsonPropertyName("coordinates")] HouseCoordinates Coordinates,
  [property: JsonPropertyName("temperature")] string Temperature,
  [property: JsonPropertyName("brightness")] string Brightness,
  [property: JsonPropertyName("humidity")] string Humidity,
  [property: JsonPropertyName("gasLevel")] string GasLevel,
  [property: JsonPropertyName("state")] HouseState State,
  [property: JsonPropertyName("sensors")] HouseSensors Sensors
);

public sealed record HouseCommandResult(
  [property: JsonPropertyName("accepted")] bool Accepted,
  [property: JsonPropertyName("message")] string? Message,
  [property: JsonPropertyName("snapshot")] HouseSnapshot? Snapshot
);
