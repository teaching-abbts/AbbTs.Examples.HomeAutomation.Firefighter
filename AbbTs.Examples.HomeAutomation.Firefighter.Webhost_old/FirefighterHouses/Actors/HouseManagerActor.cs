using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Actors.Messages;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Models;

using Akka.Actor;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Actors;

public sealed class HouseManagerActor : ReceiveActor
{
  public const string ActorName = "house-manager";
  public const string ActorPath = "/user/" + ActorName;

  private static readonly TimeSpan AskTimeout = TimeSpan.FromSeconds(5);

  private readonly Dictionary<string, IActorRef> _houses = new(StringComparer.OrdinalIgnoreCase);

  public HouseManagerActor()
  {
    foreach (var seed in HouseSeedData.Houses)
    {
      var actor = Context.ActorOf(Props.Create(() => new HouseActor(seed)), $"house-{seed.BuildingId}");
      _houses[seed.BuildingId] = actor;
    }

    Receive<IHouseCommand>(message =>
    {
      if (_houses.TryGetValue(message.BuildingId, out var actor))
      {
        actor.Forward(message);
      }
      else
      {
        Sender.Tell(new HouseCommandResult(false, $"Unknown house '{message.BuildingId}'.", null));
      }
    });

    ReceiveAsync<GetHouseSnapshot>(async message =>
    {
      var replyTo = Sender;

      if (!_houses.TryGetValue(message.BuildingId, out var actor))
      {
        replyTo.Tell(new HouseSnapshotResponse(null));
        return;
      }

      var snapshot = await actor.Ask<HouseSnapshot>(message, AskTimeout);
      replyTo.Tell(new HouseSnapshotResponse(snapshot));
    });

    ReceiveAsync<GetAllHouseSnapshots>(async _ =>
    {
      var replyTo = Sender;
      var tasks = _houses.Values.Select(actor =>
        actor.Ask<HouseSnapshot>(new GetHouseSnapshot(string.Empty), AskTimeout)
      );

      var snapshots = await Task.WhenAll(tasks);
      replyTo.Tell(
        snapshots.OrderBy(snapshot => snapshot.BuildingId, StringComparer.OrdinalIgnoreCase).ToArray()
      );
    });
  }
}
