using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Actors;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Actors.Messages;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Models;

using Akka.Actor;
using Akka.Persistence.Query;
using Akka.Persistence.Sql.Query;
using Akka.Streams;
using Akka.Streams.Dsl;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Services;

public sealed class HouseReadModel : IHouseReadModel
{
  private readonly Lazy<SqlReadJournal> _readJournal;
  private readonly Lazy<IMaterializer> _materializer;

  public HouseReadModel(ActorSystem actorSystem)
  {
    _readJournal = new Lazy<SqlReadJournal>(
      () => PersistenceQuery.Get(actorSystem).ReadJournalFor<SqlReadJournal>(SqlReadJournal.Identifier)
    );
    _materializer = new Lazy<IMaterializer>(() => actorSystem.Materializer());
  }

  public async Task<IReadOnlyList<HouseSnapshot>> GetHousesAsync(CancellationToken cancellationToken)
  {
    var snapshots = await Task.WhenAll(
      HouseSeedData.Houses.Select(seed => BuildSnapshotAsync(seed, cancellationToken))
    );

    return snapshots.OrderBy(snapshot => snapshot.BuildingId, StringComparer.OrdinalIgnoreCase).ToArray();
  }

  public async Task<HouseSnapshot?> GetHouseAsync(string buildingId, CancellationToken cancellationToken)
  {
    var seed = HouseSeedData.Houses.FirstOrDefault(house =>
      string.Equals(house.BuildingId, buildingId, StringComparison.OrdinalIgnoreCase)
    );

    return seed is null ? null : await BuildSnapshotAsync(seed, cancellationToken);
  }

  private async Task<HouseSnapshot> BuildSnapshotAsync(
    HouseSnapshot seed,
    CancellationToken cancellationToken
  )
  {
    var persistenceId = HouseActor.BuildPersistenceId(seed.BuildingId);

    var events = await _readJournal
      .Value.CurrentEventsByPersistenceId(persistenceId, 0L, long.MaxValue)
      .Select(envelope => (HouseSettingChanged)envelope.Event)
      .RunWith(Sink.Seq<HouseSettingChanged>(), _materializer.Value)
      .WaitAsync(cancellationToken);

    return events.Count == 0 ? seed : seed with { Sensors = events[^1].Sensors };
  }
}
