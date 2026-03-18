using Coderynx.Functional.Results;
using MediaBedrock.Controller.Domain.Jobs.Parameters;
using MediaBedrock.Controller.Domain.JobTemplates;
using MediaBedrock.Controller.Presentation.Jobs.Contracts;

namespace MediaBedrock.Controller.Presentation.Jobs.Mappers;

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