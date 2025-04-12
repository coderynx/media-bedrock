using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Batches;

namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

public interface IBatchJobParametersSerializer
{
    Result<string> Serialize(BatchJobParameters parameters);
    Result<BatchJobParameters> Deserialize(string serialized);
}