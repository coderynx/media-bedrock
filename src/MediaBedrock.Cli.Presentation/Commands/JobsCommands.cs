using Cocona;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Parameters;
using MediaBedrock.Cli.Domain.Jobs.Templates;
using Spectre.Console;

namespace MediaBedrock.Cli.Presentation.Commands;

public sealed class JobsCommands(IJobFactory jobFactory, IJobSerializer jobSerializer, IJobRunner jobRunner)
{
    [Command("generate")]
    public async Task Generate(
        [Argument(Name = "template", Description = "The name of the template to use for processing the job.")]
        string templateName,
        string inputs,
        string outputs,
        string properties,
        string outputPath)
    {
        var createTemplateName = JobTemplateName.Create(templateName);
        if (createTemplateName.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create template name: {createTemplateName.Error.Message}[/]");
            return;
        }

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

        var jobParameters = new JobParameters(
            TemplateName: createTemplateName.Value,
            Inputs: createInputs.Value,
            Outputs: createOutputs.Value,
            Properties: createProperties.Value);

        var createJob = await jobFactory.CreateAsync(jobParameters);
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

        var jobJson = serializedJob.Value;
        await File.WriteAllTextAsync(outputPath, jobJson);

        AnsiConsole.MarkupLine($"[green]Job generated successfully and saved to {outputPath}[/]");
    }

    [Command("take")]
    public async Task Take(
        [Argument(Name = "template", Description = "The name of the template to use for processing the job.")]
        string templateName,
        [Option("inputs", ['i'])] string inputs,
        [Option("outputs", ['o'])] string outputs,
        [Option("properties", ['p'])] string properties = "")
    {
        var createTemplateName = JobTemplateName.Create(templateName);
        if (createTemplateName.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to create template name: {createTemplateName.Error.Message}[/]");
            return;
        }

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

        var jobParameters = new JobParameters(
            TemplateName: createTemplateName.Value,
            Inputs: createInputs.Value,
            Outputs: createOutputs.Value,
            Properties: createProperties.Value);

        var createJob = await jobFactory.CreateAsync(jobParameters);
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

        var result = await jobRunner.TakeAsync(job);
        if (result.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to run the job: {result.Error.Message}[/]");
        }
    }

    [Command("take-json")]
    public async Task Take(
        [Argument(Name = "path", Description = "The path of the job manifest")]
        string path)
    {
        var jobJson = await File.ReadAllTextAsync(path);

        var deserializeJob = jobSerializer.Deserialize(jobJson);
        if (deserializeJob.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to deserialize job: {deserializeJob.Error.Message}[/]");
            return;
        }

        var result = await jobRunner.TakeAsync(deserializeJob.Value);
        if (result.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to run the job: {result.Error.Message}[/]");
        }
    }
}