using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Jobs.Steps;
using MediaBedrock.Domain.Processors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Infrastructure.Database.Configurations;

public sealed class JobRunStepEntityConfiguration : IEntityTypeConfiguration<JobRunStep>
{
    public void Configure(EntityTypeBuilder<JobRunStep> builder)
    {
        builder.HasKey(jssm => jssm.Id);

        builder.HasOne(jssm => jssm.JobRun)
            .WithMany(jsm => jsm.Steps)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(jssm => jssm.Id)
            .HasConversion(id => id.Value, value => new JobRunStepId(value))
            .ValueGeneratedNever();

        builder.Property(jssm => jssm.Order)
            .HasConversion(order => order.Value, value => new JobRunStepOrder(value))
            .IsRequired();

        builder.Property(jssm => jssm.StepName)
            .HasConversion(name => name.ToString(), value => new JobStepName(value))
            .IsRequired();

        builder.Property(jssm => jssm.ProcessorName)
            .HasConversion(name => name.ToString(), value => new ProcessorName(value))
            .IsRequired();

        builder.Property(jssm => jssm.Status)
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

        builder.OwnsMany(jssm => jssm.StepOutputs, o =>
        {
            o.Property(p => p.AssetName)
                .HasConversion(name => name.Value, value => new JobAssetName(value))
                .IsRequired();

            o.ToJson();
        });

        builder.OwnsMany(jssm => jssm.StepProperties, p => { p.ToJson(); });
    }
}