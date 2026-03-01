using System.Reflection;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Models;

public record VersionInfo
{
    public required string Name { get; init; }

    public required string SemVer { get; init; }
    public required string InformationalVersion { get; init; }

    public static VersionInfo Instance(Assembly assembly)
    {
        var gitVersionInformationType = assembly?.GetType("GitVersionInformation");
        var fields = gitVersionInformationType?.GetFields();
        var semVerField = fields?.FirstOrDefault(f => f.Name == "SemVer");
        var semVer = semVerField?.GetValue(null)?.ToString() ?? "??";
        var informationalVersion =
          fields?.FirstOrDefault(f => f.Name == "InformationalVersion")?.GetValue(null)?.ToString()
          ?? "??";

        return new VersionInfo
        {
            Name = assembly?.GetName().Name ?? "??",
            SemVer = semVer,
            InformationalVersion = informationalVersion,
        };
    }
}
