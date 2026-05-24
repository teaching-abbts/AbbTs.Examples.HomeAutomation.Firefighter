using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Build.Context;
using Build.Models;

using Cake.Core.Diagnostics;

namespace Build.Services;

public static class RuntimeOrchestrator
{
  public static void Start(BuildContext context, RuntimeMode mode)
  {
    var paths = context.GetRuntimePaths(mode);
    var settings = context.LoadSmartHomeSettings(paths.SmartHomesConfigFile);
    SmartHomeInitializer.Initialize(paths, settings.SmartHomes);

    var components = RuntimeComponentFactory.Build(paths, settings.SmartHomes);
    EnsureDirectoriesExist(components);

    var alreadyRunning = context
      .LoadTrackedProcesses(paths.ProcessesFile)
      .Where(item => ProcessService.IsProcessRunning(item.Pid))
      .ToList();

    if (alreadyRunning.Count > 0)
    {
      context.Log.Warning("There are already tracked processes running. Use stop first.");
      return;
    }

    var started = new List<TrackedProcess>();

    try
    {
      foreach (var component in components)
      {
        var process = ProcessService.StartComponent(component);
        var tracked = new TrackedProcess
        {
          Name = component.Name,
          Pid = process.Id,
          StartTimeUtc = DateTime.UtcNow.ToString("O"),
          WorkingDirectory = component.WorkingDirectory,
          Command = $"{component.FileName} {string.Join(" ", component.Arguments)}",
        };

        started.Add(tracked);
        context.Log.Information($"Started {component.Name} (PID {process.Id})");

        if (component.WaitForPort is { } port)
        {
          context.Log.Information($"Waiting for {component.Name} to listen on port {port}...");
          var ok = ProcessService.WaitForTcpPort(
            port,
            TimeSpan.FromSeconds(component.WaitTimeoutSeconds)
          );
          if (!ok)
          {
            throw new TimeoutException(
              $"{component.Name} did not open port {port} within {component.WaitTimeoutSeconds} seconds."
            );
          }

          context.Log.Information($"{component.Name} is listening on port {port}.");
        }
      }

      context.SaveTrackedProcesses(paths.ProcessesFile, started);
      context.Log.Information("All components started.");
    }
    catch
    {
      StopTracked(context, started);
      throw;
    }
  }

  public static void Stop(BuildContext context, RuntimeMode mode)
  {
    var paths = context.GetRuntimePaths(mode);
    var tracked = context.LoadTrackedProcesses(paths.ProcessesFile);
    if (tracked.Count == 0)
    {
      context.Log.Information("No tracked processes found.");
      return;
    }

    foreach (var item in tracked)
    {
      var wasRunning = ProcessService.IsProcessRunning(item.Pid);
      ProcessService.TryStop(item.Pid);

      context.Log.Information(
        wasRunning
          ? $"{item.Name}: terminated (PID {item.Pid})."
          : $"{item.Name}: PID {item.Pid} is not running."
      );
    }

    context.RemoveTrackedProcessesFile(paths.ProcessesFile);
    context.Log.Information("Stopped all tracked components.");
  }

  public static void Status(BuildContext context, RuntimeMode mode)
  {
    var paths = context.GetRuntimePaths(mode);
    var tracked = context.LoadTrackedProcesses(paths.ProcessesFile);
    if (tracked.Count == 0)
    {
      context.Log.Information("No tracked processes found.");
      return;
    }

    foreach (var item in tracked)
    {
      var isRunning = ProcessService.IsProcessRunning(item.Pid);
      context.Log.Information(
        $"{item.Name}: {(isRunning ? "running" : "stopped")} (PID {item.Pid})"
      );
    }
  }

  private static void StopTracked(BuildContext context, IReadOnlyCollection<TrackedProcess> tracked)
  {
    foreach (var item in tracked)
    {
      ProcessService.TryStop(item.Pid);
      context.Log.Information($"Stopped {item.Name} (PID {item.Pid}).");
    }
  }

  private static void EnsureDirectoriesExist(IEnumerable<RuntimeComponent> components)
  {
    foreach (var component in components)
    {
      if (!Directory.Exists(component.WorkingDirectory))
      {
        throw new DirectoryNotFoundException(
          $"Working directory '{component.WorkingDirectory}' does not exist."
        );
      }
    }
  }
}
