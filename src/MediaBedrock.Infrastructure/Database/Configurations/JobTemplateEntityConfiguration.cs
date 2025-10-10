using MediaBedrock.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Infrastructure.Database.Configurations;

public sealed class JobTemplateEntityConfiguration : IEntityTypeConfiguration<JobTemplate>
{
    public void Configure(EntityTypeBuilder<JobTemplate> builder)
    {
        builder.HasKey(jt => jt.Id);

        builder.HasIndex(jt => jt.Name)
            .IsUnique();

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

        builder.OwnsMany(jt => jt.Inputs, jti =>
        {
            jti.Property(p => p.Name)
                .IsRequired();

            jti.Property(p => p.DisplayName)
                .IsRequired(false)
                .HasMaxLength(100);

            jti.Property(p => p.Description)
                .IsRequired(false)
                .HasMaxLength(500);

            jti.ToJson();
        });

        builder.OwnsMany(jt => jt.Outputs, jto =>
        {
            jto.Property(p => p.Name)
                .IsRequired();

            jto.Property(p => p.DisplayName)
                .IsRequired(false)
                .HasMaxLength(100);

            jto.Property(p => p.Description)
                .IsRequired(false)
                .HasMaxLength(500);

            jto.ToJson();
        });

        builder.OwnsMany(jt => jt.Properties, jtp =>
        {
            jtp.Property(p => p.Name)
                .IsRequired();

            jtp.Property(p => p.DefaultValue)
                .IsRequired();

            jtp.Property(p => p.DisplayName)
                .IsRequired(false)
                .HasMaxLength(100);

            jtp.Property(p => p.Description)
                .IsRequired(false)
                .HasMaxLength(500);

            jtp.ToJson();
        });
    }
}