using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Domain.Models;

namespace Infrastructure.Data;

	public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<User>(entity =>
			{
				entity.ToTable("user");
				entity.Property(u => u.IsActive).IsRequired();
				entity.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
				entity.Property(u => u.LastName).HasMaxLength(50);
			});

			// Configure other Identity tables as needed
			builder.Entity<IdentityRole<Guid>>(entity => { entity.ToTable("role"); });
			builder.Entity<IdentityUserRole<Guid>>(entity => { entity.ToTable("user_role"); });
			builder.Entity<IdentityUserClaim<Guid>>(entity => { entity.ToTable("user_claim"); });
			builder.Entity<IdentityUserLogin<Guid>>(entity => { entity.ToTable("user_login"); });
			builder.Entity<IdentityUserToken<Guid>>(entity => { entity.ToTable("user_token"); });
			builder.Entity<IdentityRoleClaim<Guid>>(entity => { entity.ToTable("role_claim"); });
		}

		
}

