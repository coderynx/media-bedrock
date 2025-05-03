using MediaBedrock.Cli.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Cli.Infrastructure.Persistence.Configurations;

public sealed class JobEntityConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(j => j.Id);

        builder.HasOne(j => j.Template)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(j => j.Inputs, i =>
        {
            i.Property(p => p.Id)
                .HasConversion(id => id.Value, value => new JobInputId(value))
                .ValueGeneratedNever();

            i.ToJson();
        });

        builder.OwnsMany(j => j.Outputs, o =>
        {
            o.Property(p => p.Id)
                .HasConversion(id => id.Value, value => new JobOutputId(value))
                .ValueGeneratedNever();

            o.ToJson();
        });

        builder.HasMany(j => j.Steps)
            .WithOne(s => s.Job)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(j => j.Id)
            .HasConversion(id => id.Value, value => new JobId(value))
            .ValueGeneratedNever();
    }
}