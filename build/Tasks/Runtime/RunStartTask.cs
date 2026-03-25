using Build.Context;
using Build.Models;
using Build.Services;

using Cake.Frosting;

namespace Build.Tasks.Runtime;

[TaskName("Run-Start")]
public sealed class RunStartTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        RuntimeOrchestrator.Start(context, RuntimeMode.Local);
    }
}
