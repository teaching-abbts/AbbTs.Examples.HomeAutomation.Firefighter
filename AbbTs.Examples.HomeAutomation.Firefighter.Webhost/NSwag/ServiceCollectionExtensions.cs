using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Models;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.NSwag;

public static class ServiceCollectionExtensions
{
    public static void SetupNSwag(this IServiceCollection services, string title, string version)
    {
        services.AddEndpointsApiExplorer();

        services.AddOpenApiDocument(config =>
        {
            config.Title = title;
            config.Version = version;
        });
    }

    public static void SetupNSwag(this IServiceCollection services, VersionInfo versionInfo)
    {
        services.SetupNSwag(versionInfo.Name, versionInfo.SemVer);
    }

    public static void UseNSwag(this IApplicationBuilder app)
    {
        app.UseOpenApi();
        app.UseSwaggerUi();
    }
}
