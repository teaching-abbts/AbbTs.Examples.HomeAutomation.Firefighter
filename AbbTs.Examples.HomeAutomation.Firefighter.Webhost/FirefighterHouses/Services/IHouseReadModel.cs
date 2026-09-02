using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Models;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Services;

// Read side of the CQRS split: rebuilds snapshots from the persistence query journal on every call.
public interface IHouseReadModel
{
  Task<IReadOnlyList<HouseSnapshot>> GetHousesAsync(CancellationToken cancellationToken);

  Task<HouseSnapshot?> GetHouseAsync(string buildingId, CancellationToken cancellationToken);
}
