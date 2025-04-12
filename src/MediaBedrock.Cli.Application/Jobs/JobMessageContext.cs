using MediaBedrock.Cli.Domain.Jobs;

namespace MediaBedrock.Cli.Application.Jobs;

public sealed record JobMessageContext<TJobMessage>(
    JobStateMachine JobStateMachine,
    TJobMessage JobMessage) where TJobMessage : JobMessage;