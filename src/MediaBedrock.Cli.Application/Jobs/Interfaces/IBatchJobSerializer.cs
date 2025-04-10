using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Batches;

namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

public interface IBatchJobSerializer
{
    Result<string> Serialize(BatchJob batchJob);
    Result<BatchJob> Deserialize(string serialized);
}