using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Assets;

namespace MediaBedrock.Cli.Application.Jobs.Handlers;

public sealed record RunJob : JobMessage
{
    public DateTime StartedAt { get; init; } = DateTime.UtcNow;

    public static RunJob Create(JobStateMachine jobStateMachine)
    {
        var startJob = new RunJob
        {
            JobId = jobStateMachine.JobId,
            StartedAt = DateTime.UtcNow
        };

        return startJob;
    }
}

public sealed class RunJobHandler(IJobMessageBus messageBus) : IJobMessageHandler<RunJob>
{
    public async Task HandleAsync(JobMessageContext<RunJob> context, CancellationToken ct = default)
    {
        var inputAssets = context.JobStateMachine.AssetsPool.ResolveAssets(JobAssetKind.Input);

        var jobSteps = context.JobStateMachine.JobActivities
            .Where(s => s.Step.Sinks.Any(a => inputAssets.Any(i => i.Name.Equals(a.AssetName))))
            .ToList();

        context.JobStateMachine.UpdateStatus(JobWorkflowStatus.Running);

        foreach (var startJobStep in jobSteps.Select(jobStep => new ProcessJobStep
                 {
                     JobId = context.JobMessage.JobId,
                     StepName = jobStep.Step.Name
                 }))
            await messageBus.PublishAsync(startJobStep, ct);
    }
}