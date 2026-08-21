using Build.Context;
using Build.Models;
using Build.Services;

using Cake.Frosting;

namespace Build.Tasks.Runtime;

[TaskName("Run-Prepare-SmartHomes")]
public sealed class RunPrepareSmartHomesTask : FrostingTask<BuildContext>
{
  public override void Run(BuildContext context)
  {
    RuntimeOrchestrator.Prepare(context, RuntimeMode.Local);
  }
}