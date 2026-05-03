using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors.Messages;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;

using Akka.Actor;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors;

public sealed class SmartHomeActor : ReceiveActor, ICancelable
{
  private const int MaxEnvelopeCount = 100;

  private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);
  private readonly LinkedList<SmartHomeGatewayEnvelope> _recentEnvelopes = new();

  private SmartHomeRegistration _registration;
  private WebSocket? _webSocket;
  private DateTime? _connectedAtUtc;
  private DateTime? _lastSeenUtc;

  public bool IsCancellationRequested { get; }
  public CancellationToken Token { get; }

  public SmartHomeActor(SmartHomeRegistration registration)
  {
    _registration = registration;

    Receive<AttachDeviceSocket>(message =>
    {
      var previousSocket = _webSocket;
      _webSocket = message.WebSocket;
      _connectedAtUtc ??= DateTime.UtcNow;
      _lastSeenUtc = DateTime.UtcNow;

      if (previousSocket is not null && !ReferenceEquals(previousSocket, message.WebSocket))
      {
        _ = ClosePreviousSocketAsync(previousSocket);
      }
    });

    Receive<RecordSmartHomeEnvelope>(message =>
    {
      RecordEnvelope(message.Envelope);
    });

    Receive<MarkDeviceDisconnected>(message =>
    {
      if (ReferenceEquals(_webSocket, message.WebSocket))
      {
        _webSocket = null;
      }

      _lastSeenUtc = DateTime.UtcNow;
    });

    Receive<GetSmartHomeSummary>(_ =>
    {
      Sender.Tell(CreateSummary());
    });

    Receive<GetSmartHomeDetails>(_ =>
    {
      Sender.Tell(CreateDetails());
    });

    ReceiveAsync<SendSmartHomeDashboardCommand>(HandleDashboardCommandAsync);
  }

  private async Task HandleDashboardCommandAsync(SendSmartHomeDashboardCommand message)
  {
    var replyTo = Sender;
    var socket = _webSocket;

    if (socket is not { State: WebSocketState.Open })
    {
      replyTo.Tell(new SmartHomeCommandResult(false, "No smart-home is currently connected."));
      return;
    }

    await SendTextAsync(socket, message.Command.MessageType, Token);

    if (message.Command.Payload is { } payload)
    {
      await SendTextAsync(socket, payload.GetRawText(), Token);
    }

    RecordEnvelope(
      new SmartHomeGatewayEnvelope(
        "outbound " + message.Command.MessageType,
        message.Command.Payload,
        DateTime.UtcNow
      )
    );

    replyTo.Tell(new SmartHomeCommandResult(true, null));
  }

  private SmartHomeSummary CreateSummary()
  {
    return new SmartHomeSummary(
      _registration.BuildingId,
      _registration.Owner,
      _registration.XCoordinate,
      _registration.YCoordinate,
      _webSocket is { State: WebSocketState.Open },
      _connectedAtUtc,
      _lastSeenUtc,
      _recentEnvelopes.Count
    );
  }

  private SmartHomeDetails CreateDetails()
  {
    return new SmartHomeDetails(
      _registration.BuildingId,
      _registration.Owner,
      _registration.XCoordinate,
      _registration.YCoordinate,
      _webSocket is { State: WebSocketState.Open },
      _connectedAtUtc,
      _lastSeenUtc,
      _recentEnvelopes.ToList()
    );
  }

  private void RecordEnvelope(SmartHomeGatewayEnvelope envelope)
  {
    _lastSeenUtc = envelope.ReceivedAtUtc;
    _recentEnvelopes.AddFirst(envelope);

    while (_recentEnvelopes.Count > MaxEnvelopeCount)
    {
      _recentEnvelopes.RemoveLast();
    }

    if (
      string.Equals(envelope.MessageType, "send registration", StringComparison.OrdinalIgnoreCase)
      && envelope.Payload is { } payload
    )
    {
      var updatedRegistration = payload.Deserialize<SmartHomeRegistration>(_jsonOptions);
      if (updatedRegistration is not null)
      {
        _registration = updatedRegistration;
      }
    }
  }

  private static Task SendTextAsync(
    WebSocket socket,
    string message,
    CancellationToken cancellationToken
  )
  {
    var bytes = Encoding.UTF8.GetBytes(message);
    return socket.SendAsync(
      new ArraySegment<byte>(bytes),
      WebSocketMessageType.Text,
      true,
      cancellationToken
    );
  }

  private static async Task ClosePreviousSocketAsync(WebSocket socket)
  {
    try
    {
      if (socket.State == WebSocketState.Open)
      {
        await socket.CloseAsync(
          WebSocketCloseStatus.NormalClosure,
          "Replaced by newer session",
          CancellationToken.None
        );
      }
    }
    catch (WebSocketException) { }
  }

  public void Cancel()
  {
    throw new NotImplementedException();
  }

  public void CancelAfter(TimeSpan delay)
  {
    throw new NotImplementedException();
  }

  public void CancelAfter(int millisecondsDelay)
  {
    throw new NotImplementedException();
  }

  public void Cancel(bool throwOnFirstException)
  {
    throw new NotImplementedException();
  }
}
