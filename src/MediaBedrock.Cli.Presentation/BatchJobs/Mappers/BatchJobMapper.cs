using Coderynx.Functional.Results;
using MediaBedrock.Cli.Presentation.BatchJobs.Contracts;
using MediaBedrock.Cli.Presentation.Jobs.Mappers;
using MediaBedrock.Domain.BatchJobs;

namespace MediaBedrock.Cli.Presentation.BatchJobs.Mappers;

public static class BatchJobMapper
{
    public static Result<BatchJobParameters> ToDomain(this BatchJobParametersDto dto)
    {
        var createBatchJopParameters = dto.Entries
            .Select(entry => entry.ToDomain())
            .ToArray();

        var error = createBatchJopParameters.FirstOrDefault(r => r.IsFailure);
        if (error is not null)
        {
            return error.Error;
        }

        var parameters = new BatchJobParameters
        {
            Entries = createBatchJopParameters.Select(r => r.Value).ToArray()
        };

        return Result.Created(parameters);
    }
}