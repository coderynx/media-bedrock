using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Infrastructure.Database.Configurations;

public sealed class JobEntityConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(j => j.Id);

        builder.HasOne<JobTemplate>()
            .WithMany()
            .HasForeignKey(j => j.TemplateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(j => j.Steps)
            .WithOne(s => s.Job)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(j => j.Id)
            .HasConversion(id => id.Value, value => new JobId(value))
            .ValueGeneratedNever();

        builder.Property(j => j.TemplateId)
            .HasConversion(id => id.Value, value => new JobTemplateId(value))
            .IsRequired();

        builder.OwnsMany(j => j.Inputs, i =>
        {
            i.Property(p => p.Name).IsRequired();
            i.Property(p => p.Uri).IsRequired();

            i.ToJson();
        });

        builder.OwnsMany(j => j.Outputs, o =>
        {
            o.Property(p => p.Name).IsRequired();
            o.Property(p => p.FilePath).IsRequired();

            o.ToJson();
        });
    }
}