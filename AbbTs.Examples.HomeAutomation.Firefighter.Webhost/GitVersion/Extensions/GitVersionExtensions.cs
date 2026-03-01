using System.Reflection;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Models;


namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Extensions;

public static class GitVersionExtensions
{
    public static VersionInfo GetVersionInfo(this Assembly assembly)
    {
        return VersionInfo.Instance(assembly);
    }
}
