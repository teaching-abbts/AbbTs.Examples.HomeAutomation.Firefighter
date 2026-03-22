using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

public sealed class SmartHomeGateway(ILogger<SmartHomeGateway> logger) : ISmartHomeGateway
{
    private readonly ConcurrentDictionary<Guid, WebSocket> _dashboardSockets = new();
    private readonly SemaphoreSlim _smartHomeSendLock = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    private readonly Lock _smartHomeLock = new();
    private WebSocket? _smartHomeSocket;

    public bool IsSmartHomeConnected
    {
        get
        {
            lock (_smartHomeLock)
            {
                return _smartHomeSocket is { State: WebSocketState.Open };
            }
        }
    }

    public async Task HandleSmartHomeSessionAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        lock (_smartHomeLock)
        {
            _smartHomeSocket = webSocket;
        }

        logger.LogInformation("SmartHome WebSocket connected.");
        await BroadcastSystemStatusAsync(cancellationToken);

        try
        {
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
                await BroadcastEnvelopeAsync(envelope, cancellationToken);
            }
        }
        finally
        {
            lock (_smartHomeLock)
            {
                if (ReferenceEquals(_smartHomeSocket, webSocket))
                {
                    _smartHomeSocket = null;
                }
            }

            logger.LogInformation("SmartHome WebSocket disconnected.");
            await BroadcastSystemStatusAsync(cancellationToken);
        }
    }

    public async Task HandleDashboardSessionAsync(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var socketId = Guid.NewGuid();
        _dashboardSockets.TryAdd(socketId, webSocket);
        logger.LogInformation("Dashboard WebSocket connected with id {SocketId}.", socketId);

        await BroadcastToSingleDashboardAsync(webSocket, BuildSystemStatusEnvelope(), cancellationToken);

        try
        {
            while (webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var messageText = await ReceiveTextAsync(webSocket, cancellationToken);
                if (messageText is null)
                {
                    break;
                }

                if (!TryParseDashboardCommand(messageText, out var command))
                {
                    await SendDashboardErrorAsync(webSocket, "Invalid dashboard message format.", cancellationToken);
                    continue;
                }

                try
                {
                    await HandleDashboardCommandAsync(command!, cancellationToken);
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogWarning(ex, "Dashboard command rejected.");
                    await SendDashboardErrorAsync(webSocket, ex.Message, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Dashboard command failed unexpectedly.");
                    await SendDashboardErrorAsync(webSocket, "Command failed unexpectedly.", cancellationToken);
                }
            }
        }
        finally
        {
            _dashboardSockets.TryRemove(socketId, out _);
            logger.LogInformation("Dashboard WebSocket disconnected with id {SocketId}.", socketId);
        }
    }

    private async Task HandleDashboardCommandAsync(SmartHomeDashboardCommand command, CancellationToken cancellationToken)
    {
        switch (command.MessageType.Trim().ToLowerInvariant())
        {
            case "get state":
                await SendToSmartHomeAsync("get state", null, cancellationToken);
                break;
            case "get measurement":
                await SendToSmartHomeAsync("get measurement", null, cancellationToken);
                break;
            case "send command":
                var smartHomeCommand = command.Payload?.Deserialize<SmartHomeCommand>(_jsonOptions)
                    ?? throw new InvalidOperationException("send command requires a payload.");

                await SendToSmartHomeAsync("send command", smartHomeCommand, cancellationToken);
                break;
            default:
                throw new InvalidOperationException($"Unsupported dashboard command '{command.MessageType}'.");
        }
    }

    private async Task SendToSmartHomeAsync(string messageType, object? payload, CancellationToken cancellationToken)
    {
        WebSocket? smartHome;
        lock (_smartHomeLock)
        {
            smartHome = _smartHomeSocket;
        }

        if (smartHome is not { State: WebSocketState.Open })
        {
            throw new InvalidOperationException("No SmartHome WebSocket is currently connected.");
        }

        await _smartHomeSendLock.WaitAsync(cancellationToken);
        try
        {
            await SendTextAsync(smartHome, messageType, cancellationToken);

            if (payload is not null)
            {
                var payloadJson = JsonSerializer.Serialize(payload, _jsonOptions);
                await SendTextAsync(smartHome, payloadJson, cancellationToken);
            }

            var outbound = new SmartHomeGatewayEnvelope(
                "outbound " + messageType,
                payload is null ? null : ParsePayload(JsonSerializer.Serialize(payload, _jsonOptions)),
                DateTime.UtcNow);

            await BroadcastEnvelopeAsync(outbound, cancellationToken);
        }
        finally
        {
            _smartHomeSendLock.Release();
        }
    }

    private async Task BroadcastSystemStatusAsync(CancellationToken cancellationToken)
    {
        await BroadcastEnvelopeAsync(BuildSystemStatusEnvelope(), cancellationToken);
    }

    private Task SendDashboardErrorAsync(WebSocket socket, string message, CancellationToken cancellationToken)
    {
        return BroadcastToSingleDashboardAsync(
            socket,
            new SmartHomeGatewayEnvelope(
                "error",
                ParsePayload($"{{\"message\":\"{JsonEncodedText.Encode(message).ToString()}\"}}"),
                DateTime.UtcNow),
            cancellationToken);
    }

    private SmartHomeGatewayEnvelope BuildSystemStatusEnvelope()
    {
        var statusPayload = ParsePayload($"{{\"smartHomeConnected\":{IsSmartHomeConnected.ToString().ToLowerInvariant()}}}");
        return new SmartHomeGatewayEnvelope("system status", statusPayload, DateTime.UtcNow);
    }

    private async Task BroadcastEnvelopeAsync(SmartHomeGatewayEnvelope envelope, CancellationToken cancellationToken)
    {
        var disconnected = new List<Guid>();
        foreach (var entry in _dashboardSockets)
        {
            if (entry.Value.State != WebSocketState.Open)
            {
                disconnected.Add(entry.Key);
                continue;
            }

            try
            {
                await BroadcastToSingleDashboardAsync(entry.Value, envelope, cancellationToken);
            }
            catch (WebSocketException)
            {
                disconnected.Add(entry.Key);
            }
        }

        foreach (var socketId in disconnected)
        {
            _dashboardSockets.TryRemove(socketId, out _);
        }
    }

    private async Task BroadcastToSingleDashboardAsync(WebSocket socket, SmartHomeGatewayEnvelope envelope, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(envelope, _jsonOptions);
        await SendTextAsync(socket, json, cancellationToken);
    }

    private static async Task SendTextAsync(WebSocket socket, string message, CancellationToken cancellationToken)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
    }

    private static async Task<string?> ReceiveTextAsync(WebSocket socket, CancellationToken cancellationToken)
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

    private bool TryParseDashboardCommand(string messageText, out SmartHomeDashboardCommand? command)
    {
        command = null;

        try
        {
            command = JsonSerializer.Deserialize<SmartHomeDashboardCommand>(messageText, _jsonOptions);
            return command is not null && !string.IsNullOrWhiteSpace(command.MessageType);
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
