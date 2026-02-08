namespace Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Models;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("review");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Content);

        builder.Property(r => r.Rating)
            .IsRequired()
            .HasPrecision(3, 2);

        // Foreign key to Course
        builder.HasOne(r => r.Course)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.CourseId);
        builder.HasOne(r => r.Video)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.VideoId);

        // Foreign key to User
        builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId);

        builder.OwnsOne(r => r.Status, status =>
        {
            status.Property(s => s.CreatedAt);
            status.Property(s => s.DeletedAt);
            status.Property(s => s.IsSoftDeleted);
        });
    }
}
