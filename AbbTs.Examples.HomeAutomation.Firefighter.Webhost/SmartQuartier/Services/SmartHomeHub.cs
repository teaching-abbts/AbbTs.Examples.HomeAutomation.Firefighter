using System.Text.Json;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;

using Microsoft.AspNetCore.SignalR;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

public sealed class SmartHomeHub(ISmartHomeGateway gateway) : Hub
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static string GetGroupName(string smartHomeId) => $"smart-home:{smartHomeId}";

    public async Task Subscribe(string smartHomeId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(smartHomeId));

        var snapshot = await gateway.GetSmartHomeAsync(smartHomeId, Context.ConnectionAborted);
        if (snapshot is not null)
        {
            await Clients.Caller.SendAsync("smartHomeUpdated", snapshot, Context.ConnectionAborted);
        }
    }

    public Task Unsubscribe(string smartHomeId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(smartHomeId));
    }

    public Task RequestState(string smartHomeId)
    {
        return gateway.SendDashboardCommandAsync(
            smartHomeId,
            new SmartHomeDashboardCommand("get state", null),
            Context.ConnectionAborted);
    }

    public Task RequestMeasurement(string smartHomeId)
    {
        return gateway.SendDashboardCommandAsync(
            smartHomeId,
            new SmartHomeDashboardCommand("get measurement", null),
            Context.ConnectionAborted);
    }

    public Task SendCommand(string smartHomeId, SmartHomeCommand command)
    {
        return gateway.SendDashboardCommandAsync(
            smartHomeId,
            new SmartHomeDashboardCommand("send command", JsonSerializer.SerializeToElement(command, JsonOptions)),
            Context.ConnectionAborted);
    }
}