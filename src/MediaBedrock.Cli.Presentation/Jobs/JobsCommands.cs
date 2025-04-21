using Cocona;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Parameters;
using MediaBedrock.Cli.Presentation.Jobs.Contracts;
using MediaBedrock.Cli.Presentation.Jobs.Mappers;
using MediaBedrock.Cli.Presentation.JobTemplates.Contracts;
using MediaBedrock.Cli.Presentation.JobTemplates.Mappers;
using Spectre.Console;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MediaBedrock.Cli.Presentation.Jobs;

public sealed class JobsCommands(
    IJobFactory factory,
    IJobRunner runner)
{
    [Command("take")]
    public async Task Take(
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

        var templateJson = await File.ReadAllTextAsync(templatePath);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();

        var jobTemplateDto = deserializer.Deserialize<JobTemplateDto>(templateJson);

        var toDomainTemplate = jobTemplateDto.ToDomain();
        if (toDomainTemplate.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to convert parse JobTemplate: {toDomainTemplate.Error.Message}[/]");
            return;
        }

        var template = toDomainTemplate.Value;

        List<JobInputParameter> inputParameters = [];
        List<JobOutputParameter> outputParameters = [];
        List<JobPropertyParameter> propertyParameters = [];

        if (!string.IsNullOrWhiteSpace(parametersPath))
        {
            if (!File.Exists(parametersPath))
            {
                AnsiConsole.MarkupLine($"[red]Parameters file not found: {parametersPath}[/]");
                return;
            }

            var parametersYaml = await File.ReadAllTextAsync(parametersPath);
            var jobParametersDto = deserializer.Deserialize<JobParametersDto>(parametersYaml);

            var toDomainParameters = jobParametersDto.ToDomain();
            if (toDomainParameters.IsFailure)
            {
                AnsiConsole.MarkupLine($"[red]Failed to parse JobParameters: {toDomainParameters.Error.Message}[/]");
                return;
            }

            inputParameters.AddRange(toDomainParameters.Value.Inputs);
            outputParameters.AddRange(toDomainParameters.Value.Outputs);
            propertyParameters.AddRange(toDomainParameters.Value.Properties);
        }

        var createInputs = JobInputParameter.CreateMultiple(inputs);
        if (createInputs.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create input parameters: {createInputs.Error.Message}[/]");
            return;
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
            return;
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
            return;
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

        var jobParameters = new JobParameters(
            TemplateName: template.Name,
            Inputs: inputParameters.ToArray(),
            Outputs: outputParameters.ToArray(),
            Properties: propertyParameters.ToArray());

        var createJob = factory.Create(template, jobParameters);
        if (createJob.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create job: {createJob.Error.Message}[/]");
            return;
        }

        var job = createJob.Value;

        var result = await runner.TakeAsync(job);
        if (result.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to run the job: {result.Error.Message}[/]");
        }
    }
}