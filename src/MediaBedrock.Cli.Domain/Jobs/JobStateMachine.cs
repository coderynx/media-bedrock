using MediaBedrock.Cli.Domain.Jobs.Assets;
using MediaBedrock.Cli.Domain.Jobs.Steps;

namespace MediaBedrock.Cli.Domain.Jobs;

public enum JobActivityStatus
{
    Pending,
    Running,
    Completed,
    Failed
}

public enum JobWorkflowStatus
{
    Pending,
    Running,
    Completed,
    Failed
}

public sealed class JobActivity
{
    private JobActivity()
    {
    }

    public required Guid Id { get; init; }
    public required JobStep Step { get; init; }
    public JobActivityStatus Status { get; private set; }

    public static JobActivity Create(JobStep step)
    {
        return new JobActivity
        {
            Id = Guid.CreateVersion7(),
            Step = step,
            Status = JobActivityStatus.Pending
        };
    }

    public void UpdateStatus(JobActivityStatus status)
    {
        Status = status;
    }
}

public sealed class JobStateMachine(
    JobId jobId,
    List<JobActivity> activities,
    JobAssetsPool assetsPool)
    : IDisposable
{
    public JobId JobId { get; init; } = jobId;
    public JobWorkflowStatus Status { get; private set; } = JobWorkflowStatus.Pending;
    public List<JobActivity> JobActivities { get; init; } = activities;
    public JobAssetsPool AssetsPool { get; init; } = assetsPool;

    public void Dispose()
    {
        var temporaryDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", JobId.ToString());
        Directory.Delete(temporaryDirectory, true);
    }

    public void UpdateStatus(JobWorkflowStatus status)
    {
        Status = status;
    }
}