namespace MediaBedrock.Contracts.JobRuns;

public sealed record JobRunStepStartedIntegrationEvent(
    Guid JobRunStepId,
    string ProcessorName,
    List<JobRunStepStartedIntegrationEvent.Input> Inputs,
    List<JobRunStepStartedIntegrationEvent.Output> Outputs,
    List<JobRunStepStartedIntegrationEvent.Property> Properties)
{
    public sealed record Input(string Name, Uri Uri, string MediaInformation);

    public sealed record Output(string Name, Uri Uri);

    public sealed record Property(string Name, string? Value);
}