using MediaBedrock.Cli.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Cli.Infrastructure.Persistence.Configurations;

public sealed class JobTemplateEntityConfiguration : IEntityTypeConfiguration<JobTemplate>
{
    public void Configure(EntityTypeBuilder<JobTemplate> builder)
    {
        builder.HasKey(jt => jt.Id);

        builder.OwnsMany(jt => jt.Inputs, s => s.ToJson());
        builder.OwnsMany(jt => jt.Outputs, s => s.ToJson());
        builder.OwnsMany(jt => jt.Properties, s => s.ToJson());

        builder.HasMany(jt => jt.Jobs)
            .WithOne(j => j.Template)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(jt => jt.Steps)
            .WithOne(jts => jts.Template)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(jt => jt.Id)
            .HasConversion(id => id.Value, value => new JobTemplateId(value))
            .ValueGeneratedNever();

        builder.Property(jt => jt.Name)
            .HasConversion(name => name.Value, value => new JobTemplateName(value))
            .IsRequired();

        builder.Property(jt => jt.Author)
            .HasConversion(author => author.Value, value => new JobTemplateAuthor(value));

        builder.Property(jt => jt.Version)
            .HasConversion(version => version.Value, value => new JobTemplateVersion(value));

        builder.Property(jt => jt.DisplayName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(jt => jt.Description)
            .HasMaxLength(500)
            .IsRequired(false);
    }
}