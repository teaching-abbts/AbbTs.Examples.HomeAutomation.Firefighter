using Build.Context;
using Build.Services;

using Cake.Frosting;

namespace Build.Tasks.Artifacts;

[TaskName("Artifacts-Clean")]
public sealed class ArtifactsCleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        ArtifactBuilder.Clean(context);
    }
}
