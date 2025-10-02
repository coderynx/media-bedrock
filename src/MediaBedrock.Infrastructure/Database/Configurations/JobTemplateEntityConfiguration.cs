using MediaBedrock.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Infrastructure.Database.Configurations;

public sealed class JobTemplateEntityConfiguration : IEntityTypeConfiguration<JobTemplate>
{
    public void Configure(EntityTypeBuilder<JobTemplate> builder)
    {
        builder.HasKey(jt => jt.Id);

        builder.OwnsMany(jt => jt.Inputs, s => s.ToJson());
        builder.OwnsMany(jt => jt.Outputs, s => s.ToJson());
        builder.OwnsMany(jt => jt.Properties, s => s.ToJson());

        builder.HasMany(jt => jt.Steps)
            .WithOne(jts => jts.Template)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(jt => jt.Id)
            .HasConversion(id => id.Value, value => new JobTemplateId(value))
            .ValueGeneratedNever();

        builder.Property(jt => jt.Name)
            .HasConversion(name => name.Value, value => new JobTemplateName(value))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(jt => jt.Author)
            .HasConversion(author => author.Value, value => new JobTemplateAuthor(value))
            .HasMaxLength(100);

        builder.Property(jt => jt.Version)
            .HasConversion(version => version.Value, value => new JobTemplateVersion(value))
            .HasMaxLength(20);

        builder.Property(jt => jt.DisplayName)
            .HasConversion(name => name.Value, value => new JobTemplateDisplayName(value))
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(jt => jt.Description)
            .HasConversion(description => description.Value, value => new JobTemplateDescription(value))
            .HasMaxLength(500)
            .IsRequired(false);
    }
}