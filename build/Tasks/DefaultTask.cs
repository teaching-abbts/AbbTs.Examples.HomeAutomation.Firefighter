using Build.Tasks.Runtime;

using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Default")]
[IsDependentOn(typeof(RunStatusTask))]
public sealed class DefaultTask : FrostingTask
{
}
