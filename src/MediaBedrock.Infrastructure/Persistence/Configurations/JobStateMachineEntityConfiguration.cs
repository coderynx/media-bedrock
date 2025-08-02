using MediaBedrock.Domain.JobStateMachines;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Infrastructure.Persistence.Configurations;

public sealed class JobStateMachineEntityConfiguration : IEntityTypeConfiguration<JobStateMachine>
{
    public void Configure(EntityTypeBuilder<JobStateMachine> builder)
    {
        builder.HasKey(jsm => jsm.Id);

        builder.HasOne(jsm => jsm.Job)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(jsm => jsm.ExecutionError, e =>
        {
            e.Property(p => p.FailureReason)
                .HasConversion<string>()
                .IsRequired();

            e.Property(p => p.Message)
                .HasMaxLength(500)
                .IsRequired(false);
        });

        builder.HasMany(jsm => jsm.AssetsPool)
            .WithOne(ja => ja.JobStateMachine)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(jsm => jsm.Id)
            .HasConversion(id => id.Value, value => new JobStateMachineId(value))
            .ValueGeneratedNever();

        builder.Property(jsm => jsm.ExecutionStatus)
            .HasConversion<string>();
    }
}