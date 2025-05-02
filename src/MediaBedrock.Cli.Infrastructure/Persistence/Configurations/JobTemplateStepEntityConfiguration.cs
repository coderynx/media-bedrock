using MediaBedrock.Cli.Domain.JobTemplates;
using MediaBedrock.Cli.Domain.Processors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Cli.Infrastructure.Persistence.Configurations;

public sealed class JobTemplateStepEntityConfiguration : IEntityTypeConfiguration<JobTemplateStep>
{
    public void Configure(EntityTypeBuilder<JobTemplateStep> builder)
    {
        builder.HasKey(jts => jts.Id);

        builder.Property(jts => jts.Id)
            .HasConversion(id => id.Value, value => new JobTemplateStepId(value))
            .ValueGeneratedNever();

        builder.Property(jts => jts.Name)
            .HasConversion(name => name.Value, value => new JobTemplateStepName(value))
            .IsRequired();

        builder.Property(jts => jts.ProcessorName)
            .HasConversion(name => name.ToString(), value => ProcessorName.Create(value).Value);

        builder.Property(jts => jts.DisplayName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(jts => jts.Description)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.OwnsMany(jts => jts.Sinks, s => s.ToJson());
        builder.OwnsMany(jts => jts.Sources, s => s.ToJson());
        builder.OwnsMany(jts => jts.Properties, s => s.ToJson());
    }
}