using Build.Context;
using Build.Models;
using Build.Services;

using Cake.Frosting;

namespace Build.Tasks.Runtime;

[TaskName("Run-Stop")]
public sealed class RunStopTask : FrostingTask<BuildContext>
{
  public override void Run(BuildContext context)
  {
    RuntimeOrchestrator.Stop(context, RuntimeMode.Local);
  }
}
