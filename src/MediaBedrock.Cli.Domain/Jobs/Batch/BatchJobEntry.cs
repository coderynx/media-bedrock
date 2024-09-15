namespace MediaBedrock.Cli.Domain.Jobs.Batch;

public sealed class BatchJobEntry
{
    public BatchJobInputParameter[] Inputs { get; init; } = [];
    public BatchJobOutputParameter[] Outputs { get; init; } = [];
    public BatchJobPropertyParameter[] Properties { get; init; } = [];
}