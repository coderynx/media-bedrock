using Cocona;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using Spectre.Console;

namespace MediaBedrock.Cli.Presentation.Commands;

public sealed class BatchJobsCommands(
    IBatchJobSerializer batchJobSerializer,
    IBatchJobParametersSerializer batchJobParametersSerializer,
    IJobFactory jobFactory,
    IJobRunner jobRunner)
{
    [Command("generate")]
    public async Task Generate(string parametersPath, string batchJobOutputPath)
    {
        var parametersJson = await File.ReadAllTextAsync(parametersPath);

        var parameters = batchJobParametersSerializer.Deserialize(parametersJson);
        if (parameters.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to deserialize batch job parameters: {parameters.Error.Message}[/]");
            return;
        }

        var createJobs = await jobFactory.CreateAsync(parameters.Value);
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

    [Command("take")]
    public async Task Take(string path)
    {
        var batchJobJson = await File.ReadAllTextAsync(path);

        var batchJob = batchJobSerializer.Deserialize(batchJobJson);
        if (batchJob.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to deserialize batch job: {batchJob.Error.Message}[/]");
            return;
        }

        var result = await jobRunner.TakeAsync(batchJob.Value);
        if (result.IsFailure)
        {
            AnsiConsole.MarkupLine($"[red]Failed to run the batch job: {result.Error.Message}[/]");
        }
    }
}