using MediaBedrock.Worker.Domain.Processing.Entities;
using MediaBedrock.Worker.Domain.Processing.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Worker.Infrastructure.Database.Configurations;

public sealed class ProcessorInstanceEntityTypeConfiguration : IEntityTypeConfiguration<ProcessorInstance>
{
    public void Configure(EntityTypeBuilder<ProcessorInstance> builder)
    {
        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Id)
            .HasConversion(pi => pi.Value, value => new ProcessorInstanceId(value))
            .ValueGeneratedNever();

        builder.Property(pi => pi.JobRunStepId)
            .HasConversion(id => id.Value, value => new JobRunStepId(value))
            .IsRequired();
        
        builder.Property(pi => pi.ProcessorName)
            .HasConversion(name => name.ToString(), value => new ProcessorName(value))
            .IsRequired();

        builder.Property(pi => pi.Status)
            .HasConversion<string>();

        builder.OwnsMany(pi => pi.Inputs, i =>
        {
            i.Property(p => p.Name).IsRequired();
            i.Property(p => p.Uri).HasConversion(uri => uri.ToString(), value => new Uri(value));
            i.OwnsOne(p => p.AssetInformation);

            i.ToJson();
        });

        builder.OwnsMany(pi => pi.Outputs, i =>
        {
            i.Property(p => p.Name).IsRequired();
            i.Property(p => p.Uri).HasConversion(uri => uri.ToString(), value => new Uri(value));
        });

        builder.OwnsMany(pi => pi.Properties, p =>
        {
            p.Property(p => p.Name).IsRequired();
            p.Property(p => p.Value);
            p.ToJson();
        });
    }
}