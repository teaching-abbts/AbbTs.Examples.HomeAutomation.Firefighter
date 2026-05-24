using Build.Context;
using Build.Models;
using Build.Services;

using Cake.Frosting;

namespace Build.Tasks.Runtime;

[TaskName("Run-Status")]
public sealed class RunStatusTask : FrostingTask<BuildContext>
{
  public override void Run(BuildContext context)
  {
    RuntimeOrchestrator.Status(context, RuntimeMode.Local);
  }
}
