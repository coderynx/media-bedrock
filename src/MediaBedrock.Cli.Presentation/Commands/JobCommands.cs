using System.Text.Json;
using Cocona;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs;
using Spectre.Console;

namespace MediaBedrock.Cli.Presentation.Commands;

public sealed class JobCommands(
    IJobFactory jobFactory,
    IJobSerializer jobSerializer,
    IJobTemplateFactory jobTemplateFactory,
    IJobRunner jobRunner)
{
    [Command("take")]
    public async Task TakeJob(string templatePath, string outputPath, string inputs, string outputs, string properties)
    {
        var createJobTemplate = await jobTemplateFactory.CreateFromFileAsync(templatePath);
        if (createJobTemplate.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create job template: {createJobTemplate.Error.Message}[/]");
            return;
        }

        var template = createJobTemplate.Value;

        var createInputs = JobInputParameter.CreateMultiple(inputs);
        if (createInputs.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create input parameters: {createInputs.Error.Message}[/]");
            return;
        }

        var createOutputs = JobOutputParameter.CreateMultiple(outputs);
        if (createOutputs.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create output parameters: {createOutputs.Error.Message}[/]");
            return;
        }

        var createProperties = JobPropertyParameter.CreateMultiple(properties);
        if (createProperties.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create property parameters: {createProperties.Error.Message}[/]");
            return;
        }

        var createJob = jobFactory.Create(
            template: template,
            inputs: createInputs.Value,
            outputs: createOutputs.Value,
            properties: createProperties.Value);

        if (createJob.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create job: {createJob.Error.Message}[/]");
            return;
        }

        var job = createJob.Value;

        
        var serializedJob = jobSerializer.Serialize(job);
        if (serializedJob.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to serialize job: {serializedJob.Error.Message}[/]");
            return;
        }
        
        await File.WriteAllTextAsync(outputPath, serializedJob.Value);

        var result = await jobRunner.TakeAsync(job);
        if (result.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to run the job: {result.Error.Message}[/]");
        }
    }
}