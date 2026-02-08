using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasOne(p => p.Course)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.CourseId);

        builder.HasOne(p => p.User)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.UserId);

        builder.Property(p => p.Amount)
               .HasColumnType("decimal(18,2)");
        
        builder.Property(p => p.ConcurrencyStamp)
            .IsConcurrencyToken();
        
        }
}