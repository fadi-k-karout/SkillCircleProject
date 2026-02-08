namespace Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Models;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("course");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Title).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Description);
        builder.Property(c => c.Slug).HasMaxLength(100);
        builder.Property(c => c.Price)
            .HasColumnType("decimal(18,2)");

        // Foreign key to Skill
        builder.HasOne(c => c.Skill)
            .WithMany(s => s.Courses)
            .HasForeignKey(c => c.SkillId);

        // Foreign key to User
        builder.HasOne(c => c.Creator)
            .WithMany(u => u.Courses)
            .HasForeignKey(c => c.CreatorId);

        builder.OwnsOne(c => c.Status, status =>
        {
            status.Property(s => s.CreatedAt);
            status.Property(s => s.DeletedAt);
            status.Property(s => s.IsSoftDeleted);
        });
    }
}
