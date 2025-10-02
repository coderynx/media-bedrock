using MediaBedrock.Domain.JobRuns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Infrastructure.Database.Configurations;

public sealed class JobRunEntityConfiguration : IEntityTypeConfiguration<JobRun>
{
    public void Configure(EntityTypeBuilder<JobRun> builder)
    {
        builder.HasKey(jsm => jsm.Id);

        builder.HasOne(jsm => jsm.Job)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(jsm => jsm.Error, e =>
        {
            e.Property(p => p.FailureReason)
                .HasConversion<string>()
                .IsRequired();

            e.Property(p => p.Message)
                .HasMaxLength(500)
                .IsRequired(false);
        });

        builder.HasMany(jsm => jsm.AssetsPool)
            .WithOne(ja => ja.JobRun)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(jsm => jsm.Id)
            .HasConversion(id => id.Value, value => new JobRunId(value))
            .ValueGeneratedNever();

        builder.Property(jsm => jsm.Status)
            .HasConversion<string>();
    }
}