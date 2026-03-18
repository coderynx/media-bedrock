using Cocona;
using Coderynx.Functional.Results;
using MediaBedrock.Controller.Domain.BatchJobs.Interfaces;
using MediaBedrock.Controller.Domain.Jobs.Interfaces;
using MediaBedrock.Controller.Domain.JobTemplates;
using MediaBedrock.Controller.Domain.JobTemplates.Manifests;
using MediaBedrock.Controller.Presentation.BatchJobs.Contracts;
using MediaBedrock.Controller.Presentation.BatchJobs.Mappers;
using Spectre.Console;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MediaBedrock.Controller.Presentation.BatchJobs;

public sealed class BatchJobsCommands(IBatchJobSerializer batchJobSerializer, IJobFactory jobFactory)
{
    [Command("generate")]
    public async Task Generate(List<string> templatesPaths, string parametersPath, string batchJobOutputPath)
    {
        var readTemplateFromYaml = await ReadTemplatesFromYamlAsync(templatesPaths);
        if (readTemplateFromYaml.IsFailure)
        {
            AnsiConsole.MarkupLine(readTemplateFromYaml.Error.Message);
            return;
        }

        var parametersYaml = await File.ReadAllTextAsync(parametersPath);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        var parametersDto = deserializer.Deserialize<BatchJobParametersDto>(parametersYaml);

        var toParametersDomain = parametersDto.ToDomain();
        if (toParametersDomain.IsFailure)
        {
            AnsiConsole.MarkupLine($"Failed to deserialize batch job parameters: {toParametersDomain.Error}");
            return;
        }

        var createJobs = jobFactory.Create(readTemplateFromYaml.Value, toParametersDomain.Value);
        if (createJobs.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create batch job: {createJobs.Error.Message}[/]");
            return;
        }

        var batchJob = createJobs.Value;
        var batchJobJson = batchJobSerializer.Serialize(batchJob);
        if (batchJobJson.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to serialize batch job: {batchJobJson.Error.Message}[/]");
            return;
        }

        await File.WriteAllTextAsync(batchJobOutputPath, batchJobJson.Value);

        AnsiConsole.MarkupLine($"[green]Batch job generated successfully and saved to {batchJobOutputPath}[/]");
    }

    private static async Task<Result<List<JobTemplate>>> ReadTemplatesFromYamlAsync(IEnumerable<string> templatesPaths)
    {
        var templates = new List<JobTemplate>();
        foreach (var templatePath in templatesPaths)
        {
            if (!File.Exists(templatePath))
            {
                return JobTemplateErrors.ManifestNotFound(templatePath);
            }

            var templateYaml = await File.ReadAllTextAsync(templatePath);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            var templateManifest = deserializer.Deserialize<JobTemplateManifest>(templateYaml);
            var template = templateManifest.ToTemplate();

            templates.Add(template);
        }

        return Result.Created(templates);
    }
}