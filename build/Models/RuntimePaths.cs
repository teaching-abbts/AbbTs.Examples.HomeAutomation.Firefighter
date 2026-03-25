namespace Build.Models;

public sealed class RuntimePaths
{
    public required RuntimeMode Mode { get; init; }
    public required string RuntimeRoot { get; init; }
    public required string StateDir { get; init; }
    public required string ProcessesFile { get; init; }
    public required string SmartHomesConfigFile { get; init; }
    public required string SmartHomeTemplateDirectory { get; init; }
    public required string SmartHomeJarPath { get; init; }
    public required string DataServiceDirectory { get; init; }
    public required string SmartHomeInstancesRoot { get; init; }
    public string? WebhostProjectPath { get; init; }
    public string? WebhostPublishedDllPath { get; init; }

    public string GetSmartHomeInstanceDirectory(string instanceName)
    {
        return Path.Combine(SmartHomeInstancesRoot, instanceName);
    }
}
