using Build.Context;
using Build.Models;
using Build.Services;

using Cake.Frosting;

namespace Build.Tasks.Runtime;

[TaskName("Artifacts-Run-Status")]
public sealed class ArtifactsRunStatusTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        RuntimeOrchestrator.Status(context, RuntimeMode.Artifacts);
    }
}
