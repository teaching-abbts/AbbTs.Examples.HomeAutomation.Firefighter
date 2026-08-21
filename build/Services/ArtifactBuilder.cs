using System;
using System.Diagnostics;
using System.IO;

using Build.Context;

namespace Build.Services;

public static class ArtifactBuilder
{
  public static void Clean(BuildContext context)
  {
    var artifactsRoot = context.ArtifactsRoot;
    if (Directory.Exists(artifactsRoot))
    {
      Directory.Delete(artifactsRoot, recursive: true);
    }

    Directory.CreateDirectory(artifactsRoot);
  }

  public static void PublishWebhost(BuildContext context)
  {
    var outputDirectory = Path.Combine(context.ArtifactsRoot, "webhost");
    Directory.CreateDirectory(outputDirectory);

    var startInfo = new ProcessStartInfo
    {
      FileName = "dotnet",
      UseShellExecute = false,
      WorkingDirectory = context.RepoRoot,
    };

    startInfo.ArgumentList.Add("publish");
    startInfo.ArgumentList.Add(context.WebhostProjectPath);
    startInfo.ArgumentList.Add("-c");
    startInfo.ArgumentList.Add("Release");
    startInfo.ArgumentList.Add("-o");
    startInfo.ArgumentList.Add(outputDirectory);

    using var process =
      Process.Start(startInfo)
      ?? throw new InvalidOperationException("Failed to start dotnet publish process.");

    process.WaitForExit();
    if (process.ExitCode != 0)
    {
      throw new InvalidOperationException(
        $"dotnet publish failed with exit code {process.ExitCode}."
      );
    }
  }

  public static void StageRuntimeAssets(BuildContext context)
  {
    var dataServiceTarget = Path.Combine(context.ArtifactsRoot, "DataService");
    var smartHomeTarget = Path.Combine(context.ArtifactsRoot, "SmartHome");
    var buildTarget = Path.Combine(context.ArtifactsRoot, "build");

    CopyDirectory(context.DataServiceSourceAssetsDirectory, dataServiceTarget);
    CopyDirectory(context.SmartHomeSourceAssetsDirectory, smartHomeTarget);
    Directory.CreateDirectory(buildTarget);
    File.Copy(
      context.SmartHomesSourceConfigFile,
      Path.Combine(buildTarget, "smart-homes.json"),
      overwrite: true
    );
  }

  private static void CopyDirectory(string sourceDirectory, string targetDirectory)
  {
    if (!Directory.Exists(sourceDirectory))
    {
      throw new DirectoryNotFoundException($"Source directory '{sourceDirectory}' does not exist.");
    }

    Directory.CreateDirectory(targetDirectory);

    foreach (var file in Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories))
    {
      var relativePath = Path.GetRelativePath(sourceDirectory, file);
      var destinationFile = Path.Combine(targetDirectory, relativePath);
      var destinationParent =
        Path.GetDirectoryName(destinationFile)
        ?? throw new InvalidOperationException("Could not resolve destination directory.");

      Directory.CreateDirectory(destinationParent);
      File.Copy(file, destinationFile, overwrite: true);
    }
  }
}
