using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Templates;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Templates;

public sealed class JobTemplatesFileSystemRepository : IJobTemplatesRepository
{
    private readonly string _directory;
    private readonly IJobTemplateSerializer _jobTemplateSerializer;

    public JobTemplatesFileSystemRepository(IJobTemplateSerializer jobTemplateSerializer)
    {
        _jobTemplateSerializer = jobTemplateSerializer;
        _directory = Path.Combine(AppContext.BaseDirectory, "Templates");

        if (!Directory.Exists(_directory))
        {
            Directory.CreateDirectory(_directory);
        }
    }

    public async Task<Result> StoreAsync(JobTemplate jobTemplate)
    {
        try
        {
            var filePath = GetFilePath(jobTemplate.Name);

            var serialize = _jobTemplateSerializer.Serialize(jobTemplate);
            if (serialize.IsFailure)
            {
                return serialize;
            }

            await File.WriteAllTextAsync(filePath, serialize.Value);
            return Result.Created();
        }
        catch (Exception ex)
        {
            return JobTemplateErrors.StoreFailed(ex.Message);
        }
    }

    public async Task<Option<JobTemplate>> GetAsync(JobTemplateName name)
    {
        try
        {
            var filePath = GetFilePath(name);
            if (!File.Exists(filePath))
            {
                return Option<JobTemplate>.None();
            }

            var json = await File.ReadAllTextAsync(filePath);

            var deserialize = _jobTemplateSerializer.Deserialize(json);
            return deserialize.IsFailure
                ? Option<JobTemplate>.None()
                : Option<JobTemplate>.Some(deserialize.Value);
        }
        catch
        {
            return Option<JobTemplate>.None();
        }
    }

    public Task<Result> DeleteAsync(JobTemplateName name)
    {
        try
        {
            var filePath = GetFilePath(name);
            if (!File.Exists(filePath))
            {
                return Task.FromResult<Result>(JobTemplateErrors.NotFound(name));
            }

            File.Delete(filePath);
            return Task.FromResult(Result.Deleted());
        }
        catch (Exception ex)
        {
            return Task.FromResult<Result>(JobTemplateErrors.DeleteFailed(ex.Message));
        }
    }

    private string GetFilePath(JobTemplateName name)
    {
        return Path.Combine(_directory, $"{name.Value}.json");
    }
}