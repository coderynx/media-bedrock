using MediaBedrock.Cli.Domain.JobsStateMachine;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Cli.Infrastructure.Persistence.Configurations;

public sealed class JobStateMachineEntityConfiguration : IEntityTypeConfiguration<JobStateMachine>
{
    public void Configure(EntityTypeBuilder<JobStateMachine> builder)
    {
        builder.HasKey(jsm => jsm.Id);

        builder.HasOne(jsm => jsm.Job)
            .WithMany()
            .OnDelete(DeleteBehavior.Cascade);

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