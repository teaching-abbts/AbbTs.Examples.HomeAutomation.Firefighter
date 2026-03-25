using Cake.Frosting;

namespace Build.Tasks.Artifacts;

[TaskName("Artifacts-Build")]
[IsDependentOn(typeof(ArtifactsStageScriptsTask))]
public sealed class ArtifactsBuildTask : FrostingTask
{
}
