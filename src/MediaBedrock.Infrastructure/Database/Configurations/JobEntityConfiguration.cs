using MediaBedrock.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Infrastructure.Database.Configurations;

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
            i.HasKey("Id");

            i.Property<Guid>("Id")
                .ValueGeneratedOnAdd();

            i.ToJson();
        });

        builder.OwnsMany(j => j.Outputs, o =>
        {
            o.HasKey("Id");

            o.Property<Guid>("Id")
                .ValueGeneratedOnAdd();

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