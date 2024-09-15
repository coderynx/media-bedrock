namespace MediaBedrock.Cli.Domain.Jobs.Batch;

public sealed record BatchJobPropertyParameter(string Name, string Value)
{
    public JobPropertyParameter ToJobPropertyParameter()
    {
        return new JobPropertyParameter(Name, Value);
    }
}