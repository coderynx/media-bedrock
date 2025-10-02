using MediaBedrock.Domain.JobTemplates;
using MediaBedrock.Domain.Processors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Infrastructure.Database.Configurations;

public sealed class JobTemplateStepEntityConfiguration : IEntityTypeConfiguration<JobTemplateStep>
{
    public void Configure(EntityTypeBuilder<JobTemplateStep> builder)
    {
        builder.HasKey(jts => jts.Id);

        builder.Property(jts => jts.Id)
            .HasConversion(id => id.Value, value => new JobTemplateStepId(value))
            .ValueGeneratedNever();

        builder.Property(jts => jts.Order)
            .HasConversion(order => order.Value, value => new JobTemplateStepOrder(value))
            .IsRequired();

        builder.Property(jts => jts.Name)
            .HasConversion(name => name.Value, value => new JobTemplateStepName(value))
            .IsRequired();

        builder.Property(jts => jts.ProcessorName)
            .HasConversion(name => name.ToString(), value => new ProcessorName(value));

        builder.Property(jts => jts.DisplayName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(jts => jts.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.OwnsMany(jts => jts.Inputs, s => s.ToJson());
        builder.OwnsMany(jts => jts.Outputs, s => s.ToJson());
        builder.OwnsMany(jts => jts.Properties, s => s.ToJson());
    }
}