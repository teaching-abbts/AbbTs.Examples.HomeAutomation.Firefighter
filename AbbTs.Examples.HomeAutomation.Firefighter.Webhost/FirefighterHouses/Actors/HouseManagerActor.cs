using System;
using System.Collections.Generic;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Actors.Messages;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Models;

using Akka.Actor;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Actors;

public sealed class HouseManagerActor : ReceiveActor
{
  public const string ActorName = "house-manager";
  public const string ActorPath = "/user/" + ActorName;

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
  }
}
