using System.Reflection;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Models;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Extensions;

public static class ServiceCollectionExtensions
{
    public static VersionInfo RegisterVersionInfo(
      this IServiceCollection services,
      Assembly? assembly = null
    )
    {
        assembly ??= Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

        var versionInfo = assembly.GetVersionInfo();

        services.AddSingleton(versionInfo);
        return versionInfo;
    }
}
