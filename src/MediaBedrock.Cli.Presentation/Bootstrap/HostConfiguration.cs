using Cocona;
using Cocona.Builder;
using MediaBedrock.Cli.Presentation.Commands;

namespace MediaBedrock.Cli.Presentation.Bootstrap;

public static class HostConfiguration
{
    public static void UsePresentation(this ICoconaCommandsBuilder builder)
    {
        builder.AddSubCommand("jobs", command => { command.AddCommands<JobsCommands>(); });
        builder.AddSubCommand("batch-jobs", command => { command.AddCommands<BatchJobsCommands>(); });
        builder.AddSubCommand("job-templates", command => { command.AddCommands<JobTemplatesCommands>(); });
    }
}