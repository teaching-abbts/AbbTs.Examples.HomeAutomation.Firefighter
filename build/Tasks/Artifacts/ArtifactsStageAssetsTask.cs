using Build.Context;
using Build.Services;

using Cake.Frosting;

namespace Build.Tasks.Artifacts;

[TaskName("Artifacts-Stage-Assets")]
[IsDependentOn(typeof(ArtifactsPublishWebhostTask))]
public sealed class ArtifactsStageAssetsTask : FrostingTask<BuildContext>
{
  public override void Run(BuildContext context)
  {
    ArtifactBuilder.StageRuntimeAssets(context);
  }
}
