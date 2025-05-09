using MediaBedrock.Domain.JobStateMachine;
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

        builder.OwnsMany(
            jsm => jsm.Tags,
            t => t.Property(tag => tag.Value)
                .HasConversion(value => new JobStateMachineTag(value), tag => tag.Value));

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