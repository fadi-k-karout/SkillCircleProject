using Domain.Models;

namespace Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CdnProviderConfiguration : IEntityTypeConfiguration<CdnProvider>
{
    public void Configure(EntityTypeBuilder<CdnProvider> builder)
    {
        // Specify the table name
        builder.ToTable("cdn_provider");

        // Configure properties
        builder.HasKey(cp => cp.Id); // Primary key

        builder.Property(cp => cp.ProviderName)
            .IsRequired() // Make this property required
            .HasMaxLength(100); // Set a maximum length

        builder.Property(cp => cp.ResourceType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(cp => cp.ResourceId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(cp => cp.CreatedAt)
            .IsRequired(); // Use SQL to set the default value for CreatedAt

        // Configure the one-to-many relationship with Video
        builder.HasMany(cp => cp.Videos) // Navigation property
            .WithOne();
    }
}
