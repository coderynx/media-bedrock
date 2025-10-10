using Cocona;
using Coderynx.Functional.Results;
using Coderynx.Functional.Results.Successes;
using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.JobTemplates.Interfaces;
using MediaBedrock.Cli.Presentation.Jobs.Contracts;
using MediaBedrock.Cli.Presentation.Jobs.Mappers;
using MediaBedrock.Cli.Presentation.JobTemplates.Contracts;
using MediaBedrock.Cli.Presentation.JobTemplates.Converters;
using MediaBedrock.Cli.Presentation.JobTemplates.Mappers;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.Jobs.Parameters;
using MediaBedrock.Domain.JobTemplates;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MediaBedrock.Cli.Presentation.Jobs;

public sealed class JobsCommands(IServiceScopeFactory serviceScopeFactory)
{
    private readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithTypeConverter(new ReadOnlyDictionaryStringStringYamlTypeConverter())
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .Build();

    [Command("take-file")]
    public async Task TakeFile(
        [Argument(Name = "template-path", Description = "The path of the job template")]
        string templatePath,
        [Option("inputs", ['i'])] string inputs,
        [Option("outputs", ['o'])] string outputs,
        [Option("properties", ['p'])] string properties = "",
        [Option("parameters-path")] string? parametersPath = null)
    {
        if (!File.Exists(templatePath))
        {
            AnsiConsole.MarkupLine($"[red]Template file not found: {templatePath}[/]");
            return;
        }

        var yaml = await File.ReadAllTextAsync(templatePath);

        var deserializeManifest = Result.TryCatch(
            onTry: () => _deserializer.Deserialize<JobTemplateManifestDto>(yaml),
            onSuccess: Success.Created,
            onCatch: _ => JobTemplateErrors.ManifestDeserializationFailed(templatePath));

        if (deserializeManifest.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to deserialize job template: {deserializeManifest.Error.Message}[/]");
            return;
        }

        var manifest = deserializeManifest.Value.ToDomain();

        JobTemplate? template;

        using (var scope = serviceScopeFactory.CreateScope())
        {
            var jobTemplatesService = scope.ServiceProvider.GetRequiredService<IJobTemplatesService>();

            var getTemplate = await jobTemplatesService.GetAsync(manifest.Name, manifest.Version);
            if (getTemplate.IsSome)
            {
                template = getTemplate.ValueOrThrow();
            }
            else
            {
                AnsiConsole.MarkupLine($"Creating job template: {manifest.Name}");

                var createTemplate = await jobTemplatesService.CreateAsync(manifest);
                if (createTemplate.IsFailure)
                {
                    AnsiConsole.MarkupLine($"[red]Failed to add the job template: {createTemplate.Error.Message}[/]");
                    return;
                }

                template = createTemplate.Value;
            }
        }

        var jobParameters = await CreateParametersAsync(
            templateName: template.Name,
            inputs: inputs,
            outputs: outputs,
            properties: properties,
            parametersPath: parametersPath);

        if (jobParameters is null)
        {
            AnsiConsole.MarkupLine("[red]Failed to create job parameters.[/]");
            return;
        }

        JobId? jobId;
        using (var scope = serviceScopeFactory.CreateScope())
        {
            var jobsService = scope.ServiceProvider.GetRequiredService<IJobsService>();

            var createJob = await jobsService.CreateAsync(template.Name, jobParameters);
            if (createJob.IsFailure)
            {
                AnsiConsole.MarkupLine($"[red]Failed to create the job: {createJob.Error.Message}[/]");
                return;
            }

            jobId = createJob.Value.Id;
        }

        using (var scope = serviceScopeFactory.CreateScope())
        {
            var jobRunService = scope.ServiceProvider.GetRequiredService<IJobRunService>();

            var createJobRun = await jobRunService.CreateAsync(jobId);
            if (createJobRun.IsFailure)
            {
                AnsiConsole.MarkupLine($"[red]Failed to run the job: {createJobRun.Error.Message}[/]");
            }

            var jobRunsOrchestrator = scope.ServiceProvider.GetRequiredService<IJobRunsOrchestrator>();

            var startJobRun = await jobRunsOrchestrator.StartAsync(createJobRun.Value);
            if (startJobRun.IsFailure)
            {
                AnsiConsole.MarkupLine($"[red]Failed to run the job: {startJobRun.Error.Message}[/]");
            }

            var waitForCompletion = await jobRunsOrchestrator.WaitForCompletionAsync(
                runId: createJobRun.Value,
                delayTime: TimeSpan.FromSeconds(5));

            if (waitForCompletion.IsFailure)
            {
                AnsiConsole.MarkupLine($"[red]Failed to wait for job completion: {waitForCompletion.Error.Message}[/]");
            }
        }
    }

    [Command("take")]
    public async Task Take(
        [Argument(Name = "template-name", Description = "The name of the job template")]
        string templateName,
        [Option("inputs", ['i'])] string inputs,
        [Option("outputs", ['o'])] string outputs,
        [Option("properties", ['p'])] string properties = "",
        [Option("parameters-path")] string? parametersPath = null)
    {
        var createJobTemplateName = JobTemplateName.Create(templateName);
        if (createJobTemplateName.IsFailure)
        {
            AnsiConsole.MarkupLine("Invalid template name.");
            return;
        }

        JobTemplate? template;

        using (var scope = serviceScopeFactory.CreateScope())
        {
            var jobTemplatesService = scope.ServiceProvider.GetRequiredService<IJobTemplatesService>();

            var getTemplate = await jobTemplatesService.GetAsync(createJobTemplateName.Value);
            if (!getTemplate.IsSome)
            {
                AnsiConsole.MarkupLine($"[red]Job template not found: {createJobTemplateName}[/]");
                return;
            }

            template = getTemplate.ValueOrThrow();
        }

        var jobParameters = await CreateParametersAsync(
            templateName: template.Name,
            inputs: inputs,
            outputs: outputs,
            properties: properties,
            parametersPath: parametersPath);

        if (jobParameters is null)
        {
            AnsiConsole.MarkupLine("[red]Failed to create job parameters.[/]");
            return;
        }

        JobRunId? jobRunId;

        using (var scope = serviceScopeFactory.CreateScope())
        {
            var jobsService = scope.ServiceProvider.GetRequiredService<IJobsService>();

            var createJob = await jobsService.CreateAsync(template.Name, jobParameters);
            if (createJob.IsFailure)
            {
                AnsiConsole.MarkupLine($"[red]Failed to create the job: {createJob.Error.Message}[/]");
                return;
            }

            var jobRunService = scope.ServiceProvider.GetRequiredService<IJobRunService>();

            var createJobRun = await jobRunService.CreateAsync(createJob.Value.Id);
            if (createJobRun.IsFailure)
            {
                AnsiConsole.MarkupLine($"[red]Failed to run the job: {createJobRun.Error.Message}[/]");
            }

            jobRunId = createJobRun.Value;
        }

        using (var scope = serviceScopeFactory.CreateScope())
        {
            var jobRunsOrchestrator = scope.ServiceProvider.GetRequiredService<IJobRunsOrchestrator>();

            var startJobRun = await jobRunsOrchestrator.StartAsync(jobRunId);
            if (startJobRun.IsFailure)
            {
                AnsiConsole.MarkupLine($"[red]Failed to run the job: {startJobRun.Error.Message}[/]");
            }

            var waitForCompletion = await jobRunsOrchestrator.WaitForCompletionAsync(
                runId: jobRunId,
                delayTime: TimeSpan.FromSeconds(5));

            if (waitForCompletion.IsFailure)
            {
                AnsiConsole.MarkupLine($"[red]Failed to wait for job completion: {waitForCompletion.Error.Message}[/]");
            }
        }
    }

    private async Task<JobParameters?> CreateParametersAsync(
        JobTemplateName templateName,
        string inputs,
        string outputs,
        string properties,
        string? parametersPath)
    {
        List<JobInputParameter> inputParameters = [];
        List<JobOutputParameter> outputParameters = [];
        List<JobPropertyParameter> propertyParameters = [];

        if (!string.IsNullOrWhiteSpace(parametersPath))
        {
            if (!File.Exists(parametersPath))
            {
                AnsiConsole.MarkupLine($"[red]Parameters file not found: {parametersPath}[/]");
                return null;
            }

            var parametersYaml = await File.ReadAllTextAsync(parametersPath);
            var jobParametersDto = _deserializer.Deserialize<JobParametersDto>(parametersYaml);

            var toDomainParameters = jobParametersDto.ToDomain();
            if (toDomainParameters.IsFailure)
            {
                AnsiConsole.MarkupLine($"[red]Failed to parse JobParameters: {toDomainParameters.Error.Message}[/]");
                return null;
            }

            inputParameters.AddRange(toDomainParameters.Value.Inputs);
            outputParameters.AddRange(toDomainParameters.Value.Outputs);
            propertyParameters.AddRange(toDomainParameters.Value.Properties);
        }

        var createInputs = JobInputParameter.CreateMultiple(inputs);
        if (createInputs.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create input parameters: {createInputs.Error.Message}[/]");
            return null;
        }

        foreach (var input in createInputs.Value)
        {
            var existingInput = inputParameters.FirstOrDefault(i => i.Name.Equals(input.Name));
            if (existingInput is not null)
            {
                inputParameters.Remove(existingInput);
            }

            inputParameters.Add(input);
        }

        var createOutputs = JobOutputParameter.CreateMultiple(outputs);
        if (createOutputs.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create output parameters: {createOutputs.Error.Message}[/]");
            return null;
        }

        foreach (var output in createOutputs.Value)
        {
            var existingOutput = outputParameters.FirstOrDefault(o => o.Name.Equals(output.Name));
            if (existingOutput is not null)
            {
                outputParameters.Remove(existingOutput);
            }

            outputParameters.Add(output);
        }

        var createProperties = JobPropertyParameter.CreateMultiple(properties);
        if (createProperties.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create property parameters: {createProperties.Error.Message}[/]");
            return null;
        }

        foreach (var property in createProperties.Value)
        {
            var existingProperty = propertyParameters.FirstOrDefault(p => p.Name.Equals(property.Name));
            if (existingProperty is not null)
            {
                propertyParameters.Remove(existingProperty);
            }

            propertyParameters.Add(property);
        }

        return new JobParameters(
            TemplateName: templateName,
            Inputs: inputParameters.ToArray(),
            Outputs: outputParameters.ToArray(),
            Properties: propertyParameters.ToArray());
    }
}