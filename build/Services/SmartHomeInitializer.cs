using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Build.Models;

namespace Build.Services;

public static class SmartHomeInitializer
{
  public static void Initialize(
    RuntimePaths paths,
    IReadOnlyCollection<SmartHomeInstance> smartHomes
  )
  {
    Directory.CreateDirectory(paths.StateDir);

    if (!File.Exists(paths.SmartHomeJarPath))
    {
      throw new FileNotFoundException(
        $"SmartHome jar was not found at '{paths.SmartHomeJarPath}'."
      );
    }

    var templateConfigPath = Path.Combine(paths.SmartHomeTemplateDirectory, "SmartHome.conf");
    if (!File.Exists(templateConfigPath))
    {
      throw new FileNotFoundException(
        $"SmartHome template config was not found at '{templateConfigPath}'."
      );
    }

    var template = File.ReadAllText(templateConfigPath);
    Directory.CreateDirectory(paths.SmartHomeInstancesRoot);

    foreach (var home in smartHomes)
    {
      var instanceDirectory = paths.GetSmartHomeInstanceDirectory(home.Name);
      Directory.CreateDirectory(instanceDirectory);

      var config = template;
      config = SetConfigValue(config, "BUILDING_ID", home.BuildingId);
      config = SetConfigValue(config, "OWNER", home.Owner);
      config = SetConfigValue(config, "X_COORDINATE", home.X.ToString());
      config = SetConfigValue(config, "Y_COORDINATE", home.Y.ToString());

      File.WriteAllText(Path.Combine(instanceDirectory, "SmartHome.conf"), config);
    }
  }

  private static string SetConfigValue(string config, string key, string value)
  {
    var pattern = $@"(?m)^\s*{Regex.Escape(key)}\s*=\s*.*$";
    return Regex.Replace(config, pattern, $"{key} = {value}");
  }
}
