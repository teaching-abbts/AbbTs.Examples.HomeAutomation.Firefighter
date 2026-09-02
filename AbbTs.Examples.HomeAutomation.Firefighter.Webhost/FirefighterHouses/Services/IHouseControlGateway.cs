using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Models;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Services;

public interface IHouseControlGateway
{
  Task<IReadOnlyList<HouseSnapshot>> GetHousesAsync(CancellationToken cancellationToken);

  Task<HouseSnapshot?> GetHouseAsync(string buildingId, CancellationToken cancellationToken);

  Task<HouseSnapshot> ToggleLightAsync(
    string buildingId,
    bool isLightOn,
    CancellationToken cancellationToken
  );

  Task<HouseSnapshot> ToggleDoorLockAsync(
    string buildingId,
    bool isDoorLocked,
    CancellationToken cancellationToken
  );

  Task<HouseSnapshot> ToggleHeatingAsync(
    string buildingId,
    bool isHeatingOn,
    CancellationToken cancellationToken
  );

  Task<HouseSnapshot> ToggleAlarmAsync(
    string buildingId,
    bool isAlarmOn,
    CancellationToken cancellationToken
  );

  Task<HouseSnapshot> SetLightIntensityAsync(
    string buildingId,
    int lightIntensity,
    CancellationToken cancellationToken
  );

  Task<HouseSnapshot> SetHeatingIntensityAsync(
    string buildingId,
    int heatingIntensity,
    CancellationToken cancellationToken
  );

  Task<HouseSnapshot> SetDisplayValueAsync(
    string buildingId,
    string displayValue,
    CancellationToken cancellationToken
  );
}
