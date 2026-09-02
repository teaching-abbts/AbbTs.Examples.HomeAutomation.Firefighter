using System;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Actors.Messages;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Models;

using Akka.Persistence;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Actors;

public sealed class HouseActor : ReceivePersistentActor
{
  private readonly HouseSnapshot _seed;
  private HouseSensors _sensors;

  public static string BuildPersistenceId(string buildingId) => $"house-{buildingId}";

  public override string PersistenceId => BuildPersistenceId(_seed.BuildingId);

  public HouseActor(HouseSnapshot seed)
  {
    _seed = seed;
    _sensors = seed.Sensors;

    Recover<HouseSettingChanged>(evt => _sensors = evt.Sensors);

    Command<ToggleLight>(message =>
      PersistSensorChange(_sensors with { IsLightOn = message.IsLightOn })
    );
    Command<ToggleDoorLock>(message =>
      PersistSensorChange(_sensors with { IsDoorLocked = message.IsDoorLocked })
    );
    Command<ToggleHeating>(message =>
      PersistSensorChange(_sensors with { IsHeatingOn = message.IsHeatingOn })
    );
    Command<ToggleAlarm>(message =>
      PersistSensorChange(_sensors with { IsAlarmOn = message.IsAlarmOn })
    );
    Command<SetLightIntensity>(message =>
      PersistSensorChange(
        _sensors with
        {
          LightIntensity = Math.Clamp(message.LightIntensity, 0, 1023),
        }
      )
    );
    Command<SetHeatingIntensity>(message =>
      PersistSensorChange(
        _sensors with
        {
          HeatingIntensity = Math.Clamp(message.HeatingIntensity, 0, 30),
        }
      )
    );
    Command<SetDisplayValue>(message =>
      PersistSensorChange(_sensors with { DisplayValue = message.DisplayValue.Trim().Slice(33) })
    );
  }

  private void PersistSensorChange(HouseSensors updatedSensors)
  {
    var replyTo = Sender;

    Persist(
      new HouseSettingChanged(updatedSensors),
      evt =>
      {
        _sensors = evt.Sensors;
        replyTo.Tell(CreateSnapshot(), Self);
      }
    );
  }

  private HouseSnapshot CreateSnapshot() => _seed with { Sensors = _sensors };
}

file static class StringExtensions
{
  public static string Slice(this string value, int maxLength) =>
    value.Length <= maxLength ? value : value[..maxLength];
}
