using System.Collections.Generic;
using System.Threading;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Models;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Endpoints;

// Debug-only REST view of the actor state; the frontend uses the SignalR hub exclusively.
public static class HousesEndpoint
{
  public static RouteGroupBuilder MapHousesEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/api/houses").WithTags("Houses");

    group
      .MapGet(
        "/",
        async (IHouseControlGateway gateway, CancellationToken cancellationToken) =>
          Results.Ok(await gateway.GetHousesAsync(cancellationToken))
      )
      .WithName("GetHouses")
      .Produces<IReadOnlyList<HouseSnapshot>>(StatusCodes.Status200OK);

    group
      .MapGet(
        "/{buildingId}",
        async (
          string buildingId,
          IHouseControlGateway gateway,
          CancellationToken cancellationToken
        ) =>
        {
          var house = await gateway.GetHouseAsync(buildingId, cancellationToken);
          return house is null ? Results.NotFound() : Results.Ok(house);
        }
      )
      .WithName("GetHouse")
      .Produces<HouseSnapshot>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status404NotFound);

    return group;
  }
}
