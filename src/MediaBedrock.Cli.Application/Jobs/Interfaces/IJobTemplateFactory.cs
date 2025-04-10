using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Templates;

namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

public interface IJobTemplateFactory
{
    Task<Result<JobTemplate>> CreateFromFileAsync(string templatePath);
}