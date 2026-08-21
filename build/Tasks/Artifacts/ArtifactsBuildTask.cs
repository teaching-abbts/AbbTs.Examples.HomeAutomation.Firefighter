using Cake.Frosting;

namespace Build.Tasks.Artifacts;

[TaskName("Artifacts-Build")]
[IsDependentOn(typeof(ArtifactsStageAssetsTask))]
public sealed class ArtifactsBuildTask : FrostingTask
{
}
