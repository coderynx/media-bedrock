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

        builder.HasOne(jts => jts.Template)
            .WithMany(jt => jt.Steps)
            .OnDelete(DeleteBehavior.Cascade);

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

        builder.OwnsMany(jts => jts.Inputs, i =>
        {
            i.Property(p => p.Name).IsRequired();
            i.Property(p => p.Source).IsRequired();
            i.ToJson();
        });

        builder.OwnsMany(jts => jts.Outputs, o =>
        {
            o.Property(p => p.Name).IsRequired();
            o.Property(p => p.Destination).IsRequired();
            o.ToJson();
        });

        builder.OwnsMany(jts => jts.Properties, s =>
        {
            s.Property(p => p.Name).IsRequired();
            s.Property(p => p.Value).IsRequired();
            s.ToJson();
        });
    }
}