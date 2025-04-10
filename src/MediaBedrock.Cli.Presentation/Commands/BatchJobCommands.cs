using Cocona;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using Spectre.Console;

namespace MediaBedrock.Cli.Presentation.Commands;

public sealed class BatchJobCommands(IBatchJobSerializer batchJobSerializer, IJobRunner jobRunner)
{
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