using Coderynx.Functional.Results;
using MediaBedrock.Cli.Presentation.Jobs.Contracts;
using MediaBedrock.Domain.Jobs.Parameters;
using MediaBedrock.Domain.JobTemplates;

namespace MediaBedrock.Cli.Presentation.Jobs.Mappers;

internal static class JobParametersMapper
{
    public static Result<JobParameters> ToDomain(this JobParametersDto dto)
    {
        var templateName = new JobTemplateName(dto.TemplateName);
        var inputs = dto.Inputs.Select(i => new JobInputParameter(i.Key, i.Value)).ToArray();
        var outputs = dto.Outputs.Select(o => new JobOutputParameter(o.Key, o.Value)).ToArray();
        var properties = dto.Properties.Select(p => new JobPropertyParameter(p.Key, p.Value)).ToArray();

        var parameters = new JobParameters(templateName, inputs, outputs, properties);
        return Result.Created(parameters);
    }
}