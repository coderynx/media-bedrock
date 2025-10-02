using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.Jobs.Steps;
using MediaBedrock.Domain.Processors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Infrastructure.Database.Configurations;

public sealed class JobStepEntityConfiguration : IEntityTypeConfiguration<JobStep>
{
    public void Configure(EntityTypeBuilder<JobStep> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.Job)
            .WithMany(j => j.Steps)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(s => s.Id)
            .HasConversion(id => id.Value, value => new JobStepId(value));

        builder.Property(jssm => jssm.Order)
            .HasConversion(order => order.Value, value => new JobStepOrder(value))
            .IsRequired();
        
        builder.Property(s => s.Name)
            .HasConversion(name => name.Value, value => new JobStepName(value))
            .IsRequired();

        builder.Property(s => s.ProcessorName)
            .HasConversion(name => name.ToString(), value => new ProcessorName(value))
            .IsRequired();

        builder.OwnsMany(s => s.Inputs, i =>
        {
            i.Property(p => p.AssetName)
                .HasConversion(name => name.Value, value => new JobAssetName(value))
                .IsRequired();

            i.ToJson();
        });

        builder.OwnsMany(s => s.Outputs, s =>
        {
            s.Property(p => p.AssetName)
                .HasConversion(name => name.Value, value => new JobAssetName(value))
                .IsRequired();

            s.ToJson();
        });

        builder.OwnsMany(s => s.Properties, p => p.ToJson());
    }
}