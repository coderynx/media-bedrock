namespace MediaBedrock.Cli.Domain.Jobs.Batch;

public sealed record BatchJobInputParameter
{
    public required string Name { get; init; }
    public required string Uri { get; init; }

    public JobInputParameter ToJobInputParameter()
    {
        return new JobInputParameter(Name, Uri);
    }
}