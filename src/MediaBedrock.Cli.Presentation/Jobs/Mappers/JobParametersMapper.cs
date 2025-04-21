using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Parameters;
using MediaBedrock.Cli.Domain.JobTemplates;
using MediaBedrock.Cli.Presentation.Jobs.Contracts;

namespace MediaBedrock.Cli.Presentation.Jobs.Mappers;

internal static class JobParametersMapper
{
    public static Result<JobParameters> ToDomain(this JobParametersDto dto)
    {
        var createTemplateName = JobTemplateName.Create(dto.TemplateName);
        if (createTemplateName.IsFailure)
        {
            return createTemplateName.Error;
        }

        var inputs = dto.Inputs.Select(i => new JobInputParameter(i.Key, i.Value)).ToArray();
        var outputs = dto.Outputs.Select(o => new JobOutputParameter(o.Key, o.Value)).ToArray();
        var properties = dto.Properties.Select(p => new JobPropertyParameter(p.Key, p.Value)).ToArray();

        var parameters = new JobParameters(createTemplateName.Value, inputs, outputs, properties);
        return Result.Created(parameters);
    }
}