using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Actors;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Actors.Messages;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Models;

using Akka.Actor;

using Microsoft.AspNetCore.SignalR;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Services;

public sealed class HouseControlGateway(ActorSystem actorSystem, IHubContext<HouseControlHub> hubContext)
  : IHouseControlGateway
{
  private static readonly TimeSpan AskTimeout = TimeSpan.FromSeconds(5);

  public async Task<IReadOnlyList<HouseSnapshot>> GetHousesAsync(CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    var manager = await ResolveManagerAsync();
    return await manager
      .Ask<IReadOnlyList<HouseSnapshot>>(new GetAllHouseSnapshots(), AskTimeout)
      .WaitAsync(cancellationToken);
  }

  public async Task<HouseSnapshot?> GetHouseAsync(
    string buildingId,
    CancellationToken cancellationToken
  )
  {
    cancellationToken.ThrowIfCancellationRequested();

    var manager = await ResolveManagerAsync();
    var response = await manager
      .Ask<HouseSnapshotResponse>(new GetHouseSnapshot(buildingId), AskTimeout)
      .WaitAsync(cancellationToken);

    return response.Snapshot;
  }

  public Task<HouseSnapshot> ToggleLightAsync(
    string buildingId,
    bool isLightOn,
    CancellationToken cancellationToken
  ) => SendCommandAsync(buildingId, new ToggleLight(buildingId, isLightOn), cancellationToken);

  public Task<HouseSnapshot> ToggleDoorLockAsync(
    string buildingId,
    bool isDoorLocked,
    CancellationToken cancellationToken
  ) => SendCommandAsync(buildingId, new ToggleDoorLock(buildingId, isDoorLocked), cancellationToken);

  public Task<HouseSnapshot> ToggleHeatingAsync(
    string buildingId,
    bool isHeatingOn,
    CancellationToken cancellationToken
  ) => SendCommandAsync(buildingId, new ToggleHeating(buildingId, isHeatingOn), cancellationToken);

  public Task<HouseSnapshot> ToggleAlarmAsync(
    string buildingId,
    bool isAlarmOn,
    CancellationToken cancellationToken
  ) => SendCommandAsync(buildingId, new ToggleAlarm(buildingId, isAlarmOn), cancellationToken);

  public Task<HouseSnapshot> SetLightIntensityAsync(
    string buildingId,
    int lightIntensity,
    CancellationToken cancellationToken
  ) =>
    SendCommandAsync(
      buildingId,
      new SetLightIntensity(buildingId, lightIntensity),
      cancellationToken
    );

  public Task<HouseSnapshot> SetHeatingIntensityAsync(
    string buildingId,
    int heatingIntensity,
    CancellationToken cancellationToken
  ) =>
    SendCommandAsync(
      buildingId,
      new SetHeatingIntensity(buildingId, heatingIntensity),
      cancellationToken
    );

  public Task<HouseSnapshot> SetDisplayValueAsync(
    string buildingId,
    string displayValue,
    CancellationToken cancellationToken
  ) =>
    SendCommandAsync(buildingId, new SetDisplayValue(buildingId, displayValue), cancellationToken);

  private async Task<HouseSnapshot> SendCommandAsync(
    string buildingId,
    IHouseCommand command,
    CancellationToken cancellationToken
  )
  {
    cancellationToken.ThrowIfCancellationRequested();

    var manager = await ResolveManagerAsync();
    var result = await manager.Ask<object>(command, AskTimeout).WaitAsync(cancellationToken);

    if (result is not HouseSnapshot snapshot)
    {
      var reason = (result as HouseCommandResult)?.Message ?? "The command was rejected.";
      throw new InvalidOperationException(reason);
    }

    await NotifyHouseChangedAsync(snapshot, cancellationToken);
    return snapshot;
  }

  private async Task NotifyHouseChangedAsync(HouseSnapshot snapshot, CancellationToken cancellationToken)
  {
    var allHouses = await GetHousesAsync(cancellationToken);

    await hubContext.Clients.All.SendAsync("housesChanged", allHouses, cancellationToken);
    await hubContext
      .Clients.Group(HouseControlHub.GetGroupName(snapshot.BuildingId))
      .SendAsync("houseUpdated", snapshot, cancellationToken);
  }

  private async Task<IActorRef> ResolveManagerAsync()
  {
    return await actorSystem.ActorSelection(HouseManagerActor.ActorPath).ResolveOne(AskTimeout);
  }
}
