using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;

using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Build;

public static class Program
{
    public static int Main(string[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}

public sealed class BuildContext : FrostingContext
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    public string RepoRoot { get; }
    public string StateDir { get; }
    public string ProcessesFile { get; }
    public string SmartHomesConfigFile { get; }
    public string SmartHomeTemplateDirectory { get; }
    public string SmartHomeJarPath { get; }
    public string DataServiceDirectory { get; }
    public string WebhostProjectPath { get; }
    public string SmartHomeInstancesRoot { get; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        RepoRoot = ResolveRepoRoot(context);
        StateDir = Path.Combine(RepoRoot, ".run");
        ProcessesFile = Path.Combine(StateDir, "processes.json");
        SmartHomesConfigFile = Path.Combine(RepoRoot, "build", "smart-homes.json");
        SmartHomeTemplateDirectory = Path.Combine(RepoRoot, "smart-lodge", ".assets", "SmartHome");
        SmartHomeJarPath = Path.Combine(SmartHomeTemplateDirectory, "SmartHome.jar");
        DataServiceDirectory = Path.Combine(RepoRoot, "smart-lodge", ".assets", "DataService");
        WebhostProjectPath = Path.Combine(RepoRoot, "AbbTs.Examples.HomeAutomation.Firefighter.Webhost", "AbbTs.Examples.HomeAutomation.Firefighter.Webhost.csproj");
        SmartHomeInstancesRoot = Path.Combine(StateDir, "smarthomes");
    }

    public SmartHomeRuntimeSettings LoadSmartHomeSettings()
    {
        if (!File.Exists(SmartHomesConfigFile))
        {
            throw new FileNotFoundException($"Smart-home settings file was not found at '{SmartHomesConfigFile}'.");
        }

        var json = File.ReadAllText(SmartHomesConfigFile);
        var settings = JsonSerializer.Deserialize<SmartHomeRuntimeSettings>(json, JsonOptions)
            ?? throw new InvalidOperationException($"Failed to parse settings file '{SmartHomesConfigFile}'.");

        if (settings.SmartHomes.Count == 0)
        {
            throw new InvalidOperationException("At least one smart-home instance must be defined in the settings file.");
        }

        return settings;
    }

    public void EnsureStateDirectory()
    {
        Directory.CreateDirectory(StateDir);
    }

    public List<TrackedProcess> LoadTrackedProcesses()
    {
        if (!File.Exists(ProcessesFile))
        {
            return [];
        }

        var json = File.ReadAllText(ProcessesFile);
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<TrackedProcess>>(json, JsonOptions) ?? [];
    }

    public void SaveTrackedProcesses(IReadOnlyCollection<TrackedProcess> tracked)
    {
        EnsureStateDirectory();
        var json = JsonSerializer.Serialize(tracked, JsonOptions);
        File.WriteAllText(ProcessesFile, json);
    }

    public void RemoveTrackedProcessesFile()
    {
        if (File.Exists(ProcessesFile))
        {
            File.Delete(ProcessesFile);
        }
    }

    public void InitializeSmartHomeInstances(IReadOnlyCollection<SmartHomeInstance> smartHomes)
    {
        EnsureStateDirectory();

        if (!File.Exists(SmartHomeJarPath))
        {
            throw new FileNotFoundException($"SmartHome jar was not found at '{SmartHomeJarPath}'.");
        }

        var templateConfigPath = Path.Combine(SmartHomeTemplateDirectory, "SmartHome.conf");
        if (!File.Exists(templateConfigPath))
        {
            throw new FileNotFoundException($"SmartHome template config was not found at '{templateConfigPath}'.");
        }

        var template = File.ReadAllText(templateConfigPath);
        Directory.CreateDirectory(SmartHomeInstancesRoot);

        foreach (var home in smartHomes)
        {
            var instanceDirectory = GetSmartHomeInstanceDirectory(home.Name);
            Directory.CreateDirectory(instanceDirectory);

            var config = template;
            config = SetConfigValue(config, "BUILDING_ID", home.BuildingId);
            config = SetConfigValue(config, "OWNER", home.Owner);
            config = SetConfigValue(config, "X_COORDINATE", home.X.ToString());
            config = SetConfigValue(config, "Y_COORDINATE", home.Y.ToString());

            File.WriteAllText(Path.Combine(instanceDirectory, "SmartHome.conf"), config);
        }
    }

    public string GetSmartHomeInstanceDirectory(string instanceName)
    {
        return Path.Combine(SmartHomeInstancesRoot, instanceName);
    }

    public static bool WaitForTcpPort(int port, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync("127.0.0.1", port);
                if (connectTask.Wait(TimeSpan.FromMilliseconds(250)) && client.Connected)
                {
                    return true;
                }
            }
            catch
            {
                // Keep retrying until timeout.
            }

            Thread.Sleep(300);
        }

        return false;
    }

    public static bool IsProcessRunning(int pid)
    {
        try
        {
            var process = Process.GetProcessById(pid);
            return !process.HasExited;
        }
        catch
        {
            return false;
        }
    }

    private static string SetConfigValue(string config, string key, string value)
    {
        var pattern = $@"(?m)^\s*{Regex.Escape(key)}\s*=\s*.*$";
        return Regex.Replace(config, pattern, $"{key} = {value}");
    }

    private static string ResolveRepoRoot(ICakeContext context)
    {
        var argumentRoot = context.Arguments.GetArgument("repo-root");
        if (!string.IsNullOrWhiteSpace(argumentRoot))
        {
            return Path.GetFullPath(argumentRoot);
        }

        var currentDirectory = context.Environment.WorkingDirectory.FullPath;
        if (Directory.Exists(Path.Combine(currentDirectory, "build")) &&
            File.Exists(Path.Combine(currentDirectory, "run.ps1")))
        {
            return currentDirectory;
        }

        return Path.GetFullPath(Path.Combine(currentDirectory, ".."));
    }
}

public sealed class RuntimeComponent
{
    public required string Name { get; init; }
    public required string FileName { get; init; }
    public required string WorkingDirectory { get; init; }
    public required IReadOnlyList<string> Arguments { get; init; }
    public int? WaitForPort { get; init; }
    public int WaitTimeoutSeconds { get; init; } = 30;
}

public sealed class TrackedProcess
{
    public required string Name { get; init; }
    public required int Pid { get; init; }
    public required string StartTimeUtc { get; init; }
    public required string WorkingDirectory { get; init; }
    public required string Command { get; init; }
}

public sealed class SmartHomeRuntimeSettings
{
    public List<SmartHomeInstance> SmartHomes { get; init; } = [];
}

public sealed class SmartHomeInstance
{
    public required string Name { get; init; }
    public required string BuildingId { get; init; }
    public required string Owner { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
}

[TaskName("Run-Start")]
public sealed class RunStartTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var settings = context.LoadSmartHomeSettings();
        context.InitializeSmartHomeInstances(settings.SmartHomes);

        var components = BuildComponents(context, settings.SmartHomes);

        foreach (var component in components)
        {
            if (!Directory.Exists(component.WorkingDirectory))
            {
                throw new DirectoryNotFoundException($"Working directory '{component.WorkingDirectory}' does not exist.");
            }
        }

        var alreadyRunning = context.LoadTrackedProcesses().Where(item => BuildContext.IsProcessRunning(item.Pid)).ToList();
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
                var process = StartComponent(component);
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
                    var ok = BuildContext.WaitForTcpPort(port, TimeSpan.FromSeconds(component.WaitTimeoutSeconds));
                    if (!ok)
                    {
                        throw new TimeoutException($"{component.Name} did not open port {port} within {component.WaitTimeoutSeconds} seconds.");
                    }

                    context.Log.Information($"{component.Name} is listening on port {port}.");
                }
            }

            context.SaveTrackedProcesses(started);
            context.Log.Information("All components started.");
        }
        catch
        {
            StopTracked(context, started);
            throw;
        }
    }

    private static Process StartComponent(RuntimeComponent component)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = component.FileName,
            WorkingDirectory = component.WorkingDirectory,
            UseShellExecute = true,
        };

        foreach (var argument in component.Arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        return Process.Start(startInfo)
            ?? throw new InvalidOperationException($"Failed to start process '{component.Name}'.");
    }

    private static List<RuntimeComponent> BuildComponents(BuildContext context, IReadOnlyCollection<SmartHomeInstance> smartHomes)
    {
        var components = new List<RuntimeComponent>
        {
            new()
            {
                Name = "data-service",
                FileName = "java",
                WorkingDirectory = context.DataServiceDirectory,
                Arguments = ["-jar", "SmartQuartierDataService.jar"],
            },
            new()
            {
                Name = "webhost",
                FileName = "dotnet",
                WorkingDirectory = context.RepoRoot,
                Arguments = ["run", "--project", context.WebhostProjectPath, "--", "--urls", "http://localhost:5099"],
                WaitForPort = 5099,
                WaitTimeoutSeconds = 45,
            },
        };

        components.AddRange(smartHomes.Select(home => new RuntimeComponent
        {
            Name = home.Name,
            FileName = "java",
            WorkingDirectory = context.GetSmartHomeInstanceDirectory(home.Name),
            Arguments = ["-jar", context.SmartHomeJarPath],
        }));

        return components;
    }

    private static void StopTracked(BuildContext context, IReadOnlyCollection<TrackedProcess> tracked)
    {
        foreach (var item in tracked)
        {
            TryStop(item.Pid);
            context.Log.Information($"Stopped {item.Name} (PID {item.Pid}).");
        }
    }

    private static void TryStop(int pid)
    {
        try
        {
            var process = Process.GetProcessById(pid);
            if (process.HasExited)
            {
                return;
            }

            if (process.MainWindowHandle != IntPtr.Zero)
            {
                process.CloseMainWindow();
                if (process.WaitForExit(5000))
                {
                    return;
                }
            }

            process.Kill(true);
        }
        catch
        {
            // Ignore process shutdown failures here.
        }
    }
}

[TaskName("Run-Stop")]
public sealed class RunStopTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var tracked = context.LoadTrackedProcesses();
        if (tracked.Count == 0)
        {
            context.Log.Information("No tracked processes found.");
            return;
        }

        foreach (var item in tracked)
        {
            try
            {
                var process = Process.GetProcessById(item.Pid);
                if (process.HasExited)
                {
                    context.Log.Information($"{item.Name}: PID {item.Pid} is not running.");
                    continue;
                }

                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    process.CloseMainWindow();
                    if (process.WaitForExit(5000))
                    {
                        context.Log.Information($"{item.Name}: closed gracefully (PID {item.Pid}).");
                        continue;
                    }
                }

                process.Kill(true);
                context.Log.Information($"{item.Name}: terminated (PID {item.Pid}).");
            }
            catch
            {
                context.Log.Information($"{item.Name}: PID {item.Pid} is not running.");
            }
        }

        context.RemoveTrackedProcessesFile();
        context.Log.Information("Stopped all tracked components.");
    }
}

[TaskName("Run-Status")]
public sealed class RunStatusTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var tracked = context.LoadTrackedProcesses();
        if (tracked.Count == 0)
        {
            context.Log.Information("No tracked processes found.");
            return;
        }

        foreach (var item in tracked)
        {
            var isRunning = BuildContext.IsProcessRunning(item.Pid);
            context.Log.Information($"{item.Name}: {(isRunning ? "running" : "stopped")} (PID {item.Pid})");
        }
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(RunStatusTask))]
public sealed class DefaultTask : FrostingTask
{
}