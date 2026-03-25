using Build.Context;
using Build.Services;

using Cake.Frosting;

namespace Build.Tasks.Artifacts;

[TaskName("Artifacts-Publish-Webhost")]
[IsDependentOn(typeof(ArtifactsCleanTask))]
public sealed class ArtifactsPublishWebhostTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        ArtifactBuilder.PublishWebhost(context);
    }
}
