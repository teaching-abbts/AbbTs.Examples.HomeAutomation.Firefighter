using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors.Messages;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;

using Akka.Actor;

using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

public sealed class SmartHomeGateway(
  ActorSystem actorSystem,
  IHubContext<SmartHomeHub> hubContext,
  ILogger<SmartHomeGateway> logger
) : ISmartHomeGateway
{
  private static readonly TimeSpan AskTimeout = TimeSpan.FromSeconds(5);

  private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

  public async Task HandleSmartHomeSessionAsync(
    WebSocket webSocket,
    CancellationToken cancellationToken
  )
  {
    var manager = await ResolveManagerAsync();
    string? smartHomeId = null;

    try
    {
      var registrationMessageType = await ReceiveTextAsync(webSocket, cancellationToken);
      if (
        !string.Equals(
          registrationMessageType,
          "send registration",
          StringComparison.OrdinalIgnoreCase
        )
      )
      {
        throw new InvalidOperationException(
          "The first smart-home message must be 'send registration'."
        );
      }

      var registrationPayload =
        await ReceiveTextAsync(webSocket, cancellationToken)
        ?? throw new InvalidOperationException("The smart-home registration payload was missing.");

      var registration =
        JsonSerializer.Deserialize<SmartHomeRegistration>(registrationPayload, _jsonOptions)
        ?? throw new InvalidOperationException("The smart-home registration payload was invalid.");

      smartHomeId = registration.BuildingId;
      logger.LogInformation("SmartHome WebSocket connected for {SmartHomeId}.", smartHomeId);

      await manager.Ask<Status.Success>(
        new ConnectSmartHomeSession(registration, webSocket),
        AskTimeout
      );

      await RecordEnvelopeAsync(
        manager,
        smartHomeId,
        new SmartHomeGatewayEnvelope(
          "send registration",
          ParsePayload(registrationPayload),
          DateTime.UtcNow
        ),
        cancellationToken
      );

      await NotifySmartHomeChangedAsync(smartHomeId, cancellationToken);
      await RequestDashboardHistoryBroadcastAsync(cancellationToken);

      while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
      {
        var messageType = await ReceiveTextAsync(webSocket, cancellationToken);
        if (messageType is null)
        {
          break;
        }

        JsonElement? payload = null;
        if (messageType.StartsWith("send ", StringComparison.OrdinalIgnoreCase))
        {
          var payloadText = await ReceiveTextAsync(webSocket, cancellationToken);
          if (!string.IsNullOrWhiteSpace(payloadText))
          {
            payload = ParsePayload(payloadText);
          }
        }

        var envelope = new SmartHomeGatewayEnvelope(messageType, payload, DateTime.UtcNow);
        await RecordEnvelopeAsync(manager, smartHomeId, envelope, cancellationToken);
        RecordStatisticEnvelope(smartHomeId, envelope);
        await NotifySmartHomeChangedAsync(smartHomeId, cancellationToken);
        await RequestDashboardHistoryBroadcastAsync(cancellationToken);
      }
    }
    finally
    {
      if (!string.IsNullOrWhiteSpace(smartHomeId))
      {
        manager.Tell(new DisconnectSmartHomeSession(smartHomeId, webSocket));
        logger.LogInformation("SmartHome WebSocket disconnected for {SmartHomeId}.", smartHomeId);
        await NotifySmartHomeChangedAsync(smartHomeId, cancellationToken);
        await RequestDashboardHistoryBroadcastAsync(cancellationToken);
      }
    }
  }

  public async Task<SmartQuartierHistoryResponse> GetDashboardHistoryAsync(
    CancellationToken cancellationToken
  )
  {
    cancellationToken.ThrowIfCancellationRequested();

    var analytics = await ResolveAnalyticsAsync();
    return await analytics
      .Ask<SmartQuartierHistoryResponse>(new GetSmartQuartierHistory(), AskTimeout)
      .WaitAsync(cancellationToken);
  }

  public async Task<IReadOnlyList<SmartHomeSummary>> GetSmartHomesAsync(
    CancellationToken cancellationToken
  )
  {
    cancellationToken.ThrowIfCancellationRequested();

    var manager = await ResolveManagerAsync();
    return await manager
      .Ask<IReadOnlyList<SmartHomeSummary>>(new GetSmartHomes(), AskTimeout)
      .WaitAsync(cancellationToken);
  }

  public async Task<SmartHomeDetails?> GetSmartHomeAsync(
    string smartHomeId,
    CancellationToken cancellationToken
  )
  {
    cancellationToken.ThrowIfCancellationRequested();

    var manager = await ResolveManagerAsync();
    return await manager
      .Ask<SmartHomeDetails?>(new GetSmartHomeById(smartHomeId), AskTimeout)
      .WaitAsync(cancellationToken);
  }

  public async Task SendDashboardCommandAsync(
    string smartHomeId,
    SmartHomeDashboardCommand command,
    CancellationToken cancellationToken
  )
  {
    cancellationToken.ThrowIfCancellationRequested();

    var manager = await ResolveManagerAsync();
    var result = await manager
      .Ask<SmartHomeCommandResult>(
        new SendSmartHomeDashboardCommand(smartHomeId, command),
        AskTimeout
      )
      .WaitAsync(cancellationToken);

    if (!result.Accepted)
    {
      throw new InvalidOperationException(result.Message ?? "The command was rejected.");
    }

    await NotifySmartHomeChangedAsync(smartHomeId, cancellationToken);
  }

  private static Task RecordEnvelopeAsync(
    IActorRef manager,
    string smartHomeId,
    SmartHomeGatewayEnvelope envelope,
    CancellationToken cancellationToken
  )
  {
    cancellationToken.ThrowIfCancellationRequested();
    manager.Tell(new RecordSmartHomeEnvelope(smartHomeId, envelope));
    return Task.CompletedTask;
  }

  private void RecordStatisticEnvelope(string smartHomeId, SmartHomeGatewayEnvelope envelope)
  {
    actorSystem
      .ActorSelection(SmartQuartierStatisticsActor.ActorPath)
      .Tell(new ObserveSmartQuartierEnvelope(smartHomeId, envelope));
  }

  private async Task NotifySmartHomeChangedAsync(
    string smartHomeId,
    CancellationToken cancellationToken
  )
  {
    var summaryTask = GetSmartHomesAsync(cancellationToken);
    var detailTask = GetSmartHomeAsync(smartHomeId, cancellationToken);

    await Task.WhenAll(summaryTask, detailTask);

    await hubContext.Clients.All.SendAsync(
      "smartHomesChanged",
      summaryTask.Result,
      cancellationToken
    );

    if (detailTask.Result is { } detail)
    {
      await hubContext
        .Clients.Group(SmartHomeHub.GetGroupName(smartHomeId))
        .SendAsync("smartHomeUpdated", detail, cancellationToken);
    }
  }

  private async Task RequestDashboardHistoryBroadcastAsync(CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();
    var broadcaster = await ResolveDashboardHistoryBroadcasterAsync();
    broadcaster.Tell(ScheduleDashboardHistoryBroadcast.Instance);
  }

  private async Task<IActorRef> ResolveDashboardHistoryBroadcasterAsync()
  {
    return await actorSystem
      .ActorSelection(SmartQuartierDashboardHistoryBroadcasterActor.ActorPath)
      .ResolveOne(AskTimeout);
  }

  private async Task<IActorRef> ResolveManagerAsync()
  {
    return await actorSystem.ActorSelection(SmartHomeManagerActor.ActorPath).ResolveOne(AskTimeout);
  }

  private async Task<IActorRef> ResolveAnalyticsAsync()
  {
    return await actorSystem
      .ActorSelection(SmartQuartierAnalyticsActor.ActorPath)
      .ResolveOne(AskTimeout);
  }

  private static async Task<string?> ReceiveTextAsync(
    WebSocket socket,
    CancellationToken cancellationToken
  )
  {
    var buffer = new byte[4 * 1024];
    using var output = new MemoryStream();

    while (true)
    {
      var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
      if (result.MessageType == WebSocketMessageType.Close)
      {
        if (socket.State == WebSocketState.Open)
        {
          await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationToken);
        }

        return null;
      }

      await output.WriteAsync(buffer.AsMemory(0, result.Count), cancellationToken);
      if (result.EndOfMessage)
      {
        return Encoding.UTF8.GetString(output.ToArray());
      }
    }
  }

  private static JsonElement ParsePayload(string json)
  {
    using var document = JsonDocument.Parse(json);
    return document.RootElement.Clone();
  }
}
