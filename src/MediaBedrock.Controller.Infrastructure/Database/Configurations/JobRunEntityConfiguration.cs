using MediaBedrock.Controller.Domain.JobRuns;
using MediaBedrock.Controller.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Controller.Infrastructure.Database.Configurations;

public sealed class JobRunEntityConfiguration : IEntityTypeConfiguration<JobRun>
{
    public void Configure(EntityTypeBuilder<JobRun> builder)
    {
        builder.HasKey(jsm => jsm.Id);

        builder.HasOne<Job>()
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(jsm => jsm.AssetsPool)
            .WithOne(ja => ja.JobRun)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(jsm => jsm.Id)
            .HasConversion(id => id.Value, value => new JobRunId(value))
            .ValueGeneratedNever();

        builder.Property(jsm => jsm.JobId)
            .HasConversion(id => id.Value, value => new JobId(value))
            .IsRequired();

        builder.Property(jsm => jsm.Status)
            .HasConversion<string>();

        builder.OwnsOne(jsm => jsm.Error, e =>
        {
            e.Property(p => p.FailureReason)
                .HasConversion<string>()
                .IsRequired();

            e.Property(p => p.Message)
                .HasMaxLength(500)
                .IsRequired(false);
        });
    }
}