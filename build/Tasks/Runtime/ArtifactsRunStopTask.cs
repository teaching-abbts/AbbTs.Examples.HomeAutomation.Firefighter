using Build.Context;
using Build.Models;
using Build.Services;

using Cake.Frosting;

namespace Build.Tasks.Runtime;

[TaskName("Artifacts-Run-Stop")]
public sealed class ArtifactsRunStopTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        RuntimeOrchestrator.Stop(context, RuntimeMode.Artifacts);
    }
}
