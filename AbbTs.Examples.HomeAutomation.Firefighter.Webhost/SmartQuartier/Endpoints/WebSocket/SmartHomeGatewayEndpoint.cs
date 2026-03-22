using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.WebSocket;

public static class SmartHomeGatewayEndpoint
{
    public static void MapSmartHomeGatewayEndpoints(this WebApplication app)
    {
        app.Map("/smart-home/data", async (HttpContext context, ISmartHomeGateway gateway, CancellationToken cancellationToken) =>
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Expected a WebSocket request.", cancellationToken);
                return;
            }

            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await gateway.HandleSmartHomeSessionAsync(webSocket, cancellationToken);
        });
    }
}
