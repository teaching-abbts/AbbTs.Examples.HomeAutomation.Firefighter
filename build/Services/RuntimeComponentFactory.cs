using Build.Models;

namespace Build.Services;

public static class RuntimeComponentFactory
{
    public static List<RuntimeComponent> Build(
        RuntimePaths paths,
        IReadOnlyCollection<SmartHomeInstance> smartHomes
    )
    {
        var components = new List<RuntimeComponent>
        {
            new()
            {
                Name = "data-service",
                FileName = "java",
                WorkingDirectory = paths.DataServiceDirectory,
                Arguments = ["-jar", "SmartQuartierDataService.jar"],
            },
            CreateWebhostComponent(paths),
        };

        components.AddRange(
            smartHomes.Select(home => new RuntimeComponent
            {
                Name = home.Name,
                FileName = "java",
                WorkingDirectory = paths.GetSmartHomeInstanceDirectory(home.Name),
                Arguments = ["-jar", paths.SmartHomeJarPath],
            })
        );

        return components;
    }

    private static RuntimeComponent CreateWebhostComponent(RuntimePaths paths)
    {
        return paths.Mode switch
        {
            RuntimeMode.Local => new RuntimeComponent
            {
                Name = "webhost",
                FileName = "dotnet",
                WorkingDirectory = paths.RuntimeRoot,
                Arguments =
                [
                    "run",
                    "--project",
                    paths.WebhostProjectPath
                        ?? throw new InvalidOperationException(
                            "Webhost project path is required for local runtime."
                        ),
                    "--",
                    "--urls",
                    "http://localhost:5099",
                ],
                WaitForPort = 5099,
                WaitTimeoutSeconds = 45,
            },
            RuntimeMode.Artifacts => new RuntimeComponent
            {
                Name = "webhost",
                FileName = "dotnet",
                WorkingDirectory = Path.Combine(paths.RuntimeRoot, "webhost"),
                Arguments =
                [
                    paths.WebhostPublishedDllPath
                        ?? throw new InvalidOperationException(
                            "Published webhost dll path is required for artifact runtime."
                        ),
                    "--urls",
                    "http://localhost:5099",
                ],
                WaitForPort = 5099,
                WaitTimeoutSeconds = 45,
            },
            _ => throw new ArgumentOutOfRangeException(
                nameof(paths.Mode),
                paths.Mode,
                "Unknown runtime mode."
            ),
        };
    }
}
