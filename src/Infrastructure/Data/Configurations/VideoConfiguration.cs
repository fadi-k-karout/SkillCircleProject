using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Models;

public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        builder.ToTable("video");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Title).IsRequired().HasMaxLength(100);
        builder.Property(v => v.Slug).IsRequired().HasMaxLength(150);
        builder.Property(v => v.Description).IsRequired();
        builder.Property(v => v.DurationInSeconds);
        builder.Property(v => v.ThumbnailTime);
        builder.Property(v => v.IsPrivate);
        builder.Property(v => v.IsPaid);
        builder.Property(v => v.ProviderName);
        builder.Property(v => v.ProviderVideoId);

        // Foreign key to Course
        builder.HasOne(v => v.Course)
            .WithMany(c => c.Videos)
            .HasForeignKey(v => v.CourseId);
        
        
        
        // Foreign key to Creator
        builder.HasOne(v => v.Creator)
            .WithMany(r => r.Videos)
            .HasForeignKey(v => v.CreatorId);
        


        builder.OwnsOne(v => v.Status, status =>
        {
            status.Property(s => s.CreatedAt);
            status.Property(s => s.DeletedAt);
            status.Property(s => s.IsSoftDeleted);
        });
    }
}
