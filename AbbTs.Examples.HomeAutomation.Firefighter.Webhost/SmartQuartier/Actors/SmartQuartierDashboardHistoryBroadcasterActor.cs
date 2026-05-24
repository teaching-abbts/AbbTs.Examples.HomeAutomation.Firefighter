using System;
using System.Threading.Tasks;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors.Messages;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

using Akka.Actor;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors;

public sealed class SmartQuartierDashboardHistoryBroadcasterActor : ReceiveActor, IWithTimers
{
  public const string ActorName = "smart-quartier-dashboard-history-broadcaster";
  public const string ActorPath = "/user/" + ActorName;

  private static readonly TimeSpan AskTimeout = TimeSpan.FromSeconds(5);
  private static readonly TimeSpan Debounce = TimeSpan.FromMilliseconds(200);
  private const string BroadcastTimerKey = "dashboard-history-broadcast";

  private readonly ActorSystem _actorSystem;
  private readonly IHubContext<SmartHomeHub> _hubContext;
  private readonly ILogger<SmartQuartierDashboardHistoryBroadcasterActor> _logger;

  private bool _broadcastScheduled;

  public SmartQuartierDashboardHistoryBroadcasterActor(
    ActorSystem actorSystem,
    IHubContext<SmartHomeHub> hubContext,
    ILogger<SmartQuartierDashboardHistoryBroadcasterActor> logger
  )
  {
    _actorSystem = actorSystem;
    _hubContext = hubContext;
    _logger = logger;

    Receive<ScheduleDashboardHistoryBroadcast>(_ => ScheduleBroadcast());

    ReceiveAsync<BroadcastDashboardHistory>(async _ =>
    {
      _broadcastScheduled = false;

      try
      {
        var analyticsActor = await ResolveAnalyticsAsync();
        var history = await analyticsActor.Ask<SmartQuartierHistoryResponse>(
          new GetSmartQuartierHistory(),
          AskTimeout
        );

        await _hubContext
          .Clients.Group(SmartHomeHub.DashboardGroupName)
          .SendAsync("historyUpdated", history);
      }
      catch (Exception ex)
      {
        _logger.LogWarning(ex, "Failed to broadcast dashboard history update.");
      }
    });
  }

  public ITimerScheduler Timers { get; set; } = null!;

  private void ScheduleBroadcast()
  {
    if (_broadcastScheduled)
    {
      return;
    }

    _broadcastScheduled = true;
    Timers.StartSingleTimer(BroadcastTimerKey, BroadcastDashboardHistory.Instance, Debounce);
  }

  private async Task<IActorRef> ResolveAnalyticsAsync()
  {
    return await _actorSystem
      .ActorSelection(SmartQuartierAnalyticsActor.ActorPath)
      .ResolveOne(AskTimeout);
  }

  private sealed class BroadcastDashboardHistory
  {
    public static readonly BroadcastDashboardHistory Instance = new();

    private BroadcastDashboardHistory() { }
  }
}
