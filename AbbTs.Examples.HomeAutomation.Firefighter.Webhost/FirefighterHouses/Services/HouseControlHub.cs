using System.Threading.Tasks;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Models;

using Microsoft.AspNetCore.SignalR;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Services;

public sealed class HouseControlHub(IHouseControlGateway gateway) : Hub
{
  public const string AllHousesGroupName = "houses:all";

  public static string GetGroupName(string buildingId) => $"house:{buildingId}";

  public async Task SubscribeAll()
  {
    await Groups.AddToGroupAsync(Context.ConnectionId, AllHousesGroupName);

    var houses = await gateway.GetHousesAsync(Context.ConnectionAborted);
    await Clients.Caller.SendAsync("housesChanged", houses, Context.ConnectionAborted);
  }

  public Task UnsubscribeAll() =>
    Groups.RemoveFromGroupAsync(Context.ConnectionId, AllHousesGroupName);

  public async Task Subscribe(string buildingId)
  {
    await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(buildingId));

    var house = await gateway.GetHouseAsync(buildingId, Context.ConnectionAborted);
    if (house is not null)
    {
      await Clients.Caller.SendAsync("houseUpdated", house, Context.ConnectionAborted);
    }
  }

  public Task Unsubscribe(string buildingId) =>
    Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(buildingId));

  public Task ToggleLight(string buildingId, bool isLightOn) =>
    gateway.ToggleLightAsync(buildingId, isLightOn, Context.ConnectionAborted);

  public Task ToggleDoorLock(string buildingId, bool isDoorLocked) =>
    gateway.ToggleDoorLockAsync(buildingId, isDoorLocked, Context.ConnectionAborted);

  public Task ToggleHeating(string buildingId, bool isHeatingOn) =>
    gateway.ToggleHeatingAsync(buildingId, isHeatingOn, Context.ConnectionAborted);

  public Task ToggleAlarm(string buildingId, bool isAlarmOn) =>
    gateway.ToggleAlarmAsync(buildingId, isAlarmOn, Context.ConnectionAborted);

  public Task SetLightIntensity(string buildingId, int lightIntensity) =>
    gateway.SetLightIntensityAsync(buildingId, lightIntensity, Context.ConnectionAborted);

  public Task SetHeatingIntensity(string buildingId, int heatingIntensity) =>
    gateway.SetHeatingIntensityAsync(buildingId, heatingIntensity, Context.ConnectionAborted);

  public Task SetDisplayValue(string buildingId, string displayValue) =>
    gateway.SetDisplayValueAsync(buildingId, displayValue, Context.ConnectionAborted);
}
