namespace MediaBedrock.Cli.Domain.Jobs.Batch;

public sealed record BatchJobOutputParameter(string Name, string Uri)
{
    public JobOutputParameter ToJobOutputParameter()
    {
        return new JobOutputParameter(Name, Uri);
    }
}