using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Models;


namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Endpoints.About;

public static class AboutEndpoint
{
    /// <summary>
    /// Maps the /about endpoint to return <see cref="VersionInfo"/>.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static RouteHandlerBuilder MapAboutEndpoint(this WebApplication app)
    {
        return app.MapGet("/about", (VersionInfo versionInfo) => versionInfo)
          .WithName("GetAbout")
          .WithTags("About")
          .Produces<VersionInfo>(StatusCodes.Status200OK);
    }
}
