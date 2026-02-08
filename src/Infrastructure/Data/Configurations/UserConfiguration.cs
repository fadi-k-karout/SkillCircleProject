namespace Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Models;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");
        
        builder.Property(u => u.IsActive).IsRequired();
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(u => u.LastName).HasMaxLength(50);

        // Configure the many-to-many relationship with Skill and join table
        builder.HasMany(u => u.Skills)
            .WithMany(s => s.Users)
            .UsingEntity(j => j.ToTable("user_skill"));
        
        // Configure relationships
        builder
            .HasMany(u => u.Reviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasMany(u => u.Courses)
            .WithOne(r => r.Creator)
            .HasForeignKey(r => r.CreatorId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasMany(u => u.Videos)
            .WithOne(r => r.Creator)
            .HasForeignKey(r => r.CreatorId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}
