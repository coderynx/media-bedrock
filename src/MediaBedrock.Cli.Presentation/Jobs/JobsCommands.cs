using Cocona;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Application.JobTemplates.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Parameters;
using MediaBedrock.Cli.Domain.JobTemplates;
using MediaBedrock.Cli.Presentation.Jobs.Contracts;
using MediaBedrock.Cli.Presentation.Jobs.Mappers;
using MediaBedrock.Cli.Presentation.JobTemplates.Contracts;
using MediaBedrock.Cli.Presentation.JobTemplates.Converters;
using MediaBedrock.Cli.Presentation.JobTemplates.Mappers;
using Spectre.Console;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MediaBedrock.Cli.Presentation.Jobs;

public sealed class JobsCommands(IJobsService jobsService, IJobTemplatesService jobTemplatesService)
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

        JobTemplateManifestDto? dto;
        try
        {
            dto = _deserializer.Deserialize<JobTemplateManifestDto>(yaml);
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"[red]Failed to deserialize job template: {e.Message}[/]");
            return;
        }

        var manifest = dto.ToDomain();
        var addTemplate = await jobTemplatesService.CreateAsync(manifest);

        if (addTemplate.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to add the job template: {addTemplate.Error.Message}[/]");
            return;
        }

        var jobParameters = await CreateParameters(
            templateName: addTemplate.Value.Name,
            inputs: inputs,
            outputs: outputs,
            properties: properties,
            parametersPath: parametersPath);

        if (jobParameters is null)
        {
            AnsiConsole.MarkupLine("[red]Failed to create job parameters.[/]");
            return;
        }

        var createJob = await jobsService.CreateAsync(addTemplate.Value, jobParameters);
        if (createJob.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create the job: {createJob.Error.Message}[/]");
            return;
        }

        var startJob = await jobsService.StartAsync(createJob.Value.Id);
        if (startJob.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to run the job: {startJob.Error.Message}[/]");
        }

        var waitForCompletion = await jobsService.WaitForCompletionAsync(startJob.Value, TimeSpan.FromSeconds(5));
        if (waitForCompletion.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to wait for job completion: {waitForCompletion.Error.Message}[/]");
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
        var getTemplate = await jobTemplatesService.GetAsync(templateName);
        if (!getTemplate.IsSome)
        {
            AnsiConsole.MarkupLine($"[red]Job template not found: {templateName}[/]");
            return;
        }

        var template = getTemplate.ValueOrThrow();

        var jobParameters = await CreateParameters(
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

        var createJob = await jobsService.CreateAsync(template, jobParameters);
        if (createJob.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create the job: {createJob.Error.Message}[/]");
            return;
        }

        var result = await jobsService.StartAsync(createJob.Value.Id);
        if (result.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to run the job: {result.Error.Message}[/]");
        }
    }

    private async Task<JobParameters?> CreateParameters(
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