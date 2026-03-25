using System.Text.Json;

using Build.Models;

using Cake.Core;
using Cake.Frosting;

namespace Build.Context;

public sealed class BuildContext : FrostingContext
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };

    public string RepoRoot { get; }
    public string ArtifactsRoot { get; }
    public string WebhostProjectPath { get; }
    public string SmartHomesSourceConfigFile { get; }
    public string SmartHomeSourceAssetsDirectory { get; }
    public string DataServiceSourceAssetsDirectory { get; }

    public BuildContext(ICakeContext context)
        : base(context)
    {
        RepoRoot = ResolveRepoRoot(context);
        ArtifactsRoot = Path.Combine(RepoRoot, ".artifacts");
        WebhostProjectPath = Path.Combine(RepoRoot, "AbbTs.Examples.HomeAutomation.Firefighter.Webhost", "AbbTs.Examples.HomeAutomation.Firefighter.Webhost.csproj");
        SmartHomesSourceConfigFile = Path.Combine(RepoRoot, "build", "smart-homes.json");
        SmartHomeSourceAssetsDirectory = Path.Combine(RepoRoot, "smart-lodge", ".assets", "SmartHome");
        DataServiceSourceAssetsDirectory = Path.Combine(RepoRoot, "smart-lodge", ".assets", "DataService");
    }

    public RuntimePaths GetRuntimePaths(RuntimeMode mode)
    {
        return mode switch
        {
            RuntimeMode.Local => new RuntimePaths
            {
                Mode = mode,
                RuntimeRoot = RepoRoot,
                StateDir = Path.Combine(RepoRoot, ".run"),
                ProcessesFile = Path.Combine(RepoRoot, ".run", "processes.json"),
                SmartHomesConfigFile = SmartHomesSourceConfigFile,
                SmartHomeTemplateDirectory = SmartHomeSourceAssetsDirectory,
                SmartHomeJarPath = Path.Combine(SmartHomeSourceAssetsDirectory, "SmartHome.jar"),
                DataServiceDirectory = DataServiceSourceAssetsDirectory,
                SmartHomeInstancesRoot = Path.Combine(RepoRoot, ".run", "smarthomes"),
                WebhostProjectPath = WebhostProjectPath,
            },
            RuntimeMode.Artifacts => new RuntimePaths
            {
                Mode = mode,
                RuntimeRoot = ArtifactsRoot,
                StateDir = Path.Combine(ArtifactsRoot, ".run"),
                ProcessesFile = Path.Combine(ArtifactsRoot, ".run", "processes.json"),
                SmartHomesConfigFile = Path.Combine(ArtifactsRoot, "build", "smart-homes.json"),
                SmartHomeTemplateDirectory = Path.Combine(ArtifactsRoot, "SmartHome"),
                SmartHomeJarPath = Path.Combine(ArtifactsRoot, "SmartHome", "SmartHome.jar"),
                DataServiceDirectory = Path.Combine(ArtifactsRoot, "DataService"),
                SmartHomeInstancesRoot = Path.Combine(ArtifactsRoot, ".run", "smarthomes"),
                WebhostPublishedDllPath = Path.Combine(ArtifactsRoot, "webhost", "AbbTs.Examples.HomeAutomation.Firefighter.Webhost.dll"),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown runtime mode."),
        };
    }

    public SmartHomeRuntimeSettings LoadSmartHomeSettings(string smartHomesConfigFile)
    {
        if (!File.Exists(smartHomesConfigFile))
        {
            throw new FileNotFoundException($"Smart-home settings file was not found at '{smartHomesConfigFile}'.");
        }

        var json = File.ReadAllText(smartHomesConfigFile);
        var settings = JsonSerializer.Deserialize<SmartHomeRuntimeSettings>(json, JsonOptions)
            ?? throw new InvalidOperationException($"Failed to parse settings file '{smartHomesConfigFile}'.");

        if (settings.SmartHomes.Count == 0)
        {
            throw new InvalidOperationException("At least one smart-home instance must be defined in the settings file.");
        }

        return settings;
    }

    public List<TrackedProcess> LoadTrackedProcesses(string processesFile)
    {
        if (!File.Exists(processesFile))
        {
            return [];
        }

        var json = File.ReadAllText(processesFile);

        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return JsonSerializer.Deserialize<List<TrackedProcess>>(json, JsonOptions) ?? [];
    }

    public void SaveTrackedProcesses(string processesFile, IReadOnlyCollection<TrackedProcess> tracked)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(processesFile) ?? throw new InvalidOperationException("Invalid processes file path."));
        var json = JsonSerializer.Serialize(tracked, JsonOptions);
        File.WriteAllText(processesFile, json);
    }

    public void RemoveTrackedProcessesFile(string processesFile)
    {
        if (File.Exists(processesFile))
        {
            File.Delete(processesFile);
        }
    }

    private static string ResolveRepoRoot(ICakeContext context)
    {
        var argumentRoot = context.Arguments.GetArgument("repo-root");
        if (!string.IsNullOrWhiteSpace(argumentRoot))
        {
            var candidate = Path.GetFullPath(argumentRoot);
            if (IsRepoRoot(candidate))
            {
                return candidate;
            }

            var parent = Path.GetFullPath(Path.Combine(candidate, ".."));
            if (IsRepoRoot(parent))
            {
                return parent;
            }

            return candidate;
        }

        var currentDirectory = context.Environment.WorkingDirectory.FullPath;

        if (Directory.Exists(Path.Combine(currentDirectory, "build")) &&
            File.Exists(Path.Combine(currentDirectory, "run.ps1")))
        {
            return currentDirectory;
        }

        return Path.GetFullPath(Path.Combine(currentDirectory, ".."));
    }

    private static bool IsRepoRoot(string directory)
    {
        return Directory.Exists(Path.Combine(directory, "build")) &&
               File.Exists(Path.Combine(directory, "run.ps1"));
    }
}
