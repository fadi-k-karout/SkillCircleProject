namespace Infrastructure.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Models;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("skill");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        builder.Property(s => s.Slug).HasMaxLength(100);
        builder.Property(s => s.Description);

        builder.OwnsOne(s => s.Status, status =>
        {
            status.Property(s => s.CreatedAt);
            status.Property(s => s.DeletedAt);
            status.Property(s => s.IsSoftDeleted);
        });
    }
}
