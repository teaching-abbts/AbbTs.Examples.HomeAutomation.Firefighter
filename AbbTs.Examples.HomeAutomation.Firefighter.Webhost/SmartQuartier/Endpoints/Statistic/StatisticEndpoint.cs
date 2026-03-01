using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.Statistic;

public static class StatisticEndpoint
{
    public static RouteHandlerBuilder MapSmartQuartierStatisticEndpoint(this WebApplication app)
    {
        return app.MapGet("/smart-quartier/statistic", async (ISmartQuartierClient client, CancellationToken cancellationToken) =>
            {
                var response = await client.GetStatisticDataAsync(cancellationToken);
                return Results.Ok(response);
            })
            .WithName("GetSmartQuartierStatistic")
            .WithTags("SmartQuartier")
            .Produces<SmartQuartierStatisticResponse>(StatusCodes.Status200OK);
    }
}