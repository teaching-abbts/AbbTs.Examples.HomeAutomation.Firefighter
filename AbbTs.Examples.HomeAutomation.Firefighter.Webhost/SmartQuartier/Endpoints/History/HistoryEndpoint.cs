using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.History;

public static class HistoryEndpoint
{
    public static RouteHandlerBuilder MapSmartQuartierHistoryEndpoint(this WebApplication app)
    {
        return app.MapGet("/smart-quartier/history", async (ISmartQuartierClient client, CancellationToken cancellationToken) =>
            {
                var response = await client.GetHistoryDataAsync(cancellationToken);
                return Results.Ok(response);
            })
            .WithName("GetSmartQuartierHistory")
            .WithTags("SmartQuartier")
            .Produces<SmartQuartierHistoryResponse>(StatusCodes.Status200OK);
    }
}