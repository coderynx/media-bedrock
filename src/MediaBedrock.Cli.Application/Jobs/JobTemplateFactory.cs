using Coderynx.Functional.Results;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Templates;

namespace MediaBedrock.Cli.Application.Jobs;

public sealed class JobTemplateFactory(IJobTemplateSerializer serializer) : IJobTemplateFactory
{
    public async Task<Result<JobTemplate>> CreateFromFileAsync(string templatePath)
    {
        if (!File.Exists(templatePath))
        {
            return JobTemplateErrors.NotFound(templatePath);
        }

        // TODO: This file reading should be done in the infrastructure layer.
        var templateJson = await File.ReadAllTextAsync(templatePath);

        return serializer.Deserialize(templateJson);
    }
}