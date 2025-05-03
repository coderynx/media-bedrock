using MediaBedrock.Cli.Domain.JobAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediaBedrock.Cli.Infrastructure.Persistence.Configurations;

public sealed class JobAssetEntityConfiguration : IEntityTypeConfiguration<JobAsset>
{
    public void Configure(EntityTypeBuilder<JobAsset> builder)
    {
        builder.HasKey(ja => ja.Id);

        builder.HasOne(ja => ja.JobStateMachine)
            .WithMany(jsm => jsm.AssetsPool)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(ja => ja.Id)
            .HasConversion(id => id.Value, value => new JobAssetId(value))
            .ValueGeneratedNever();

        builder.Property(ja => ja.Name)
            .HasConversion(name => name.Value, value => new JobAssetName(value))
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(ja => ja.Uri)
            .HasMaxLength(2048);

        builder.Property(ja => ja.Kind)
            .IsRequired()
            .HasConversion<string>();

        builder.OwnsOne(ja => ja.MediaInformation, mi =>
        {
            mi.Property(m => m.Format)
                .HasMaxLength(100);
        });

        builder.Ignore(ja => ja.IsAvailable);
    }
}