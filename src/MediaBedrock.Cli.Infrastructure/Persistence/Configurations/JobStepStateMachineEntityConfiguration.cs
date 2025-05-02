using MediaBedrock.Cli.Domain.JobAssets;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.Processors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Cli.Infrastructure.Persistence.Configurations;

public sealed class JobStepStateMachineEntityConfiguration : IEntityTypeConfiguration<JobStepStateMachine>
{
    public void Configure(EntityTypeBuilder<JobStepStateMachine> builder)
    {
        builder.HasKey(jsm => jsm.Id);

        builder.Property(jsm => jsm.Id)
            .HasConversion(id => id.Value, value => new JobStepStateMachineId(value))
            .ValueGeneratedNever();

        builder.Property(jsm => jsm.StepName)
            .HasConversion(name => name.ToString(), value => JobStepName.Create(value).Value)
            .IsRequired();

        builder.Property(jsm => jsm.ProcessorName)
            .HasConversion(name => name.ToString(), value => ProcessorName.Create(value).Value)
            .IsRequired();

        builder.Property(jsm => jsm.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.OwnsMany(s => s.StepSinks, i =>
        {
            i.Property(p => p.AssetName)
                .HasConversion(name => name.Value, value => new JobAssetName(value))
                .IsRequired();

            i.ToJson();
        });

        builder.OwnsMany(s => s.StepSources, s =>
        {
            s.Property(p => p.AssetName)
                .HasConversion(name => name.Value, value => new JobAssetName(value))
                .IsRequired();

            s.ToJson();
        });

        builder.OwnsMany(s => s.StepProperties, p => p.ToJson());
    }
}