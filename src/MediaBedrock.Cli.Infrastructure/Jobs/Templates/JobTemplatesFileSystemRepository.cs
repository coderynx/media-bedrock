using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Templates;
using MediaBedrock.Cli.Domain.Jobs.Templates.Interfaces;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Templates;

public sealed class JobTemplatesFileSystemRepository : IJobTemplatesRepository
{
    private readonly string _directoryPath;

    private readonly IJobTemplateSerializerProvider _serializerProvider;

    public JobTemplatesFileSystemRepository(IJobTemplateSerializerProvider serializerProvider)
    {
        _serializerProvider = serializerProvider;
        _directoryPath = Path.Combine(AppContext.BaseDirectory, "Templates");

        if (!Directory.Exists(_directoryPath))
        {
            Directory.CreateDirectory(_directoryPath);
        }
    }

    public async Task<Result> StoreAsync(JobTemplate jobTemplate)
    {
        var serializer = _serializerProvider.ResolveSerializer(JobTemplateSerializerFormat.Json);
        if (serializer.IsFailure)
        {
            return serializer;
        }

        try
        {
            var getFilePath = GetFilePath(jobTemplate.Name);
            if (getFilePath.IsFailure)
            {
                return getFilePath.Error;
            }

            var serialize = serializer.Value.Serialize(jobTemplate);
            if (serialize.IsFailure)
            {
                return serialize;
            }

            await File.WriteAllTextAsync(getFilePath.Value, serialize.Value);
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
            var getFilePath = GetFilePath(name);
            if (getFilePath.IsFailure)
            {
                return Option<JobTemplate>.None();
            }

            var serialized = await File.ReadAllTextAsync(getFilePath.Value);

            var serializer = _serializerProvider.ResolveSerializer(Path.GetExtension(getFilePath.Value));
            if (serializer.IsFailure)
            {
                return Option<JobTemplate>.None();
            }

            var deserialize = serializer.Value.Deserialize(serialized);
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
            var getFilePath = GetFilePath(name);
            if (getFilePath.IsFailure)
            {
                return Task.FromResult<Result>(getFilePath.Error);
            }

            File.Delete(getFilePath.Value);
            return Task.FromResult(Result.Deleted());
        }
        catch (Exception ex)
        {
            return Task.FromResult<Result>(JobTemplateErrors.DeleteFailed(ex.Message));
        }
    }

    private Result<string> GetFilePath(JobTemplateName name)
    {
        var yamlPath = Path.Combine(_directoryPath, $"{name.Value}.yaml");
        if (File.Exists(yamlPath))
        {
            return Result.Found(yamlPath);
        }

        var ymlPath = Path.Combine(_directoryPath, $"{name.Value}.yml");
        if (File.Exists(ymlPath))
        {
            return Result.Found(ymlPath);
        }

        var jsonPath = Path.Combine(_directoryPath, $"{name.Value}.json");
        if (File.Exists(jsonPath))
        {
            return Result.Found(jsonPath);
        }

        return JobTemplateErrors.NotFound(name);
    }
}