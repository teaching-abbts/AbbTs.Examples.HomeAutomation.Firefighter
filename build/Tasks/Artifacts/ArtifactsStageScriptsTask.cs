using Build.Context;
using Build.Services;

using Cake.Frosting;

namespace Build.Tasks.Artifacts;

[TaskName("Artifacts-Stage-Scripts")]
[IsDependentOn(typeof(ArtifactsStageAssetsTask))]
public sealed class ArtifactsStageScriptsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        ArtifactBuilder.StageRuntimeScripts(context);
    }
}
