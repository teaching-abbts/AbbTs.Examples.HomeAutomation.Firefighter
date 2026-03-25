using Build.Context;
using Build.Models;
using Build.Services;

using Cake.Frosting;

namespace Build.Tasks.Runtime;

[TaskName("Artifacts-Run-Start")]
public sealed class ArtifactsRunStartTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        RuntimeOrchestrator.Start(context, RuntimeMode.Artifacts);
    }
}
