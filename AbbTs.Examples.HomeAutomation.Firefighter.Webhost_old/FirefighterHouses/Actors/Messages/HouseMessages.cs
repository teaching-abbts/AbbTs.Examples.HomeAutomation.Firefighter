using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Models;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Actors.Messages;

// Lets HouseManagerActor route any command to the right child without a per-type switch.
public interface IHouseCommand
{
  string BuildingId { get; }
}

public sealed record ToggleLight(string BuildingId, bool IsLightOn) : IHouseCommand;

public sealed record ToggleDoorLock(string BuildingId, bool IsDoorLocked) : IHouseCommand;

public sealed record ToggleHeating(string BuildingId, bool IsHeatingOn) : IHouseCommand;

public sealed record ToggleAlarm(string BuildingId, bool IsAlarmOn) : IHouseCommand;

public sealed record SetLightIntensity(string BuildingId, int LightIntensity) : IHouseCommand;

public sealed record SetHeatingIntensity(string BuildingId, int HeatingIntensity) : IHouseCommand;

public sealed record SetDisplayValue(string BuildingId, string DisplayValue) : IHouseCommand;

public sealed record GetHouseSnapshot(string BuildingId);

public sealed record GetAllHouseSnapshots;

// Wraps the (possibly missing) lookup result since Akka messages cannot be null.
public sealed record HouseSnapshotResponse(HouseSnapshot? Snapshot);

// Persisted event: carries the full sensors record rather than one event type per field.
public sealed record HouseSettingChanged(HouseSensors Sensors);
