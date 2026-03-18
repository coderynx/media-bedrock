using Coderynx.Functional.Results;
using MediaBedrock.Controller.Domain.BatchJobs;
using MediaBedrock.Controller.Presentation.BatchJobs.Contracts;
using MediaBedrock.Controller.Presentation.Jobs.Mappers;

namespace MediaBedrock.Controller.Presentation.BatchJobs.Mappers;

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