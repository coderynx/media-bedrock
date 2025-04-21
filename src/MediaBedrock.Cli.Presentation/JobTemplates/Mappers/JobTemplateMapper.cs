using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.JobTemplates;
using MediaBedrock.Cli.Presentation.JobTemplates.Contracts;

namespace MediaBedrock.Cli.Presentation.JobTemplates.Mappers;

internal static class JobTemplateMapper
{
    public static Result<JobTemplate> ToDomain(this JobTemplateDto dto)
    {
        var createJobTemplateName = JobTemplateName.Create(dto.Name);
        if (createJobTemplateName.IsFailure)
        {
            return createJobTemplateName.Error;
        }

        var createJobTemplateVersion = JobTemplateVersion.Create(dto.Version);
        if (createJobTemplateVersion.IsFailure)
        {
            return createJobTemplateVersion.Error;
        }

        var createJobTemplateAuthor = JobTemplateAuthor.Create(dto.Author);
        if (createJobTemplateAuthor.IsFailure)
        {
            return createJobTemplateAuthor.Error;
        }

        var createSteps = dto.Steps.Select(step => step.ToDomain()).ToArray();
        if (createSteps.Any(step => step.IsFailure))
        {
            return Result.Failure<JobTemplate>(createSteps.First(step => step.IsFailure).Error);
        }

        var steps = createSteps.Select(step => step.Value).ToArray();

        var template = new JobTemplate
        {
            Name = createJobTemplateName.Value,
            Version = createJobTemplateVersion.Value,
            Author = createJobTemplateAuthor.Value,
            DisplayName = dto.DisplayName,
            Description = dto.Description,
            Properties = dto.Properties.Select(p => p.ToDomain()).ToArray(),
            Inputs = dto.Inputs.Select(i => i.ToDomain()).ToArray(),
            Outputs = dto.Outputs.Select(o => o.ToDomain()).ToArray(),
            Steps = steps
        };

        return Result.Created(template);
    }
}