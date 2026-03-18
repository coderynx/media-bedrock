using Cocona;
using Cocona.Builder;
using MediaBedrock.Controller.Presentation.BatchJobs;
using MediaBedrock.Controller.Presentation.Jobs;
using MediaBedrock.Controller.Presentation.JobTemplates;

namespace MediaBedrock.Controller.Presentation.Bootstrap;

public static class HostConfiguration
{
    public static void UseControllerPresentation(this ICoconaCommandsBuilder builder)
    {
        builder.AddSubCommand("jobs", command => { command.AddCommands<JobsCommands>(); });
        builder.AddSubCommand("batch-jobs", command => { command.AddCommands<BatchJobsCommands>(); });
        builder.AddSubCommand("job-templates", command => { command.AddCommands<JobTemplatesCommands>(); });
    }
}