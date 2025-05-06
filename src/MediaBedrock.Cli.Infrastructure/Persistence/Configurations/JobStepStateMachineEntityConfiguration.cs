using MediaBedrock.Cli.Domain.JobAssets;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.JobsStateMachine;
using MediaBedrock.Cli.Domain.Processors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Cli.Infrastructure.Persistence.Configurations;

public sealed class JobStepStateMachineEntityConfiguration : IEntityTypeConfiguration<JobStepStateMachine>
{
    public void Configure(EntityTypeBuilder<JobStepStateMachine> builder)
    {
        builder.HasKey(jssm => jssm.Id);

        builder.HasOne(jssm => jssm.JobStateMachine)
            .WithMany(jsm => jsm.StepsStateMachines)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(jssm => jssm.Id)
            .HasConversion(id => id.Value, value => new JobStepStateMachineId(value))
            .ValueGeneratedNever();

        builder.Property(jssm => jssm.StepName)
            .HasConversion(name => name.ToString(), value => new JobStepName(value))
            .IsRequired();

        builder.Property(jssm => jssm.ProcessorName)
            .HasConversion(name => name.ToString(), value => new ProcessorName(value))
            .IsRequired();

        builder.Property(jssm => jssm.ExecutionStatus)
            .HasConversion<string>()
            .IsRequired();

        builder.OwnsOne(jssm => jssm.ExecutionError, e =>
        {
            e.Property(p => p.Reason)
                .HasConversion<string>()
                .IsRequired();

            e.Property(p => p.Message)
                .HasMaxLength(500)
                .IsRequired(false);
        });

        builder.OwnsMany(jssm => jssm.StepInputs, i =>
        {
            i.Property(p => p.AssetName)
                .HasConversion(name => name.Value, value => new JobAssetName(value))
                .IsRequired();

            i.ToJson();
        });

        builder.OwnsMany(jssm => jssm.StepOutputs, s =>
        {
            s.Property(p => p.AssetName)
                .HasConversion(name => name.Value, value => new JobAssetName(value))
                .IsRequired();

            s.ToJson();
        });

        builder.OwnsMany(jssm => jssm.StepProperties, p => p.ToJson());
    }
}