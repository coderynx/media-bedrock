using Cocona;
using Cocona.Builder;
using MediaBedrock.Cli.Presentation.Commands;

namespace MediaBedrock.Cli.Presentation.Bootstrap;

public static class HostConfiguration
{
    public static void UsePresentation(this ICoconaCommandsBuilder builder)
    {
        builder.AddSubCommand("job", command => { command.AddCommands<JobCommands>(); });
        builder.AddSubCommand("batch-job", command => { command.AddCommands<BatchJobCommands>(); });
    }
}