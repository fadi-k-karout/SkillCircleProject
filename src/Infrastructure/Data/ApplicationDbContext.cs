using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Infrastructure.Data.Configurations;

namespace Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}


	public DbSet<Skill> Skills { get; set; }
	public DbSet<Course> Courses { get; set; }

	public DbSet<Video> Videos { get; set; }
	public DbSet<Review> Reviews { get; set; }
	public DbSet<Payment> Payments { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{


		builder.ApplyConfiguration(new UserConfiguration());
		builder.ApplyConfiguration(new SkillConfiguration());
		builder.ApplyConfiguration(new CourseConfiguration());
		builder.ApplyConfiguration(new VideoConfiguration());
		builder.ApplyConfiguration(new ReviewConfiguration());
		builder.ApplyConfiguration(new PaymentConfiguration());


		// Configure other Identity tables as needed
		builder.Entity<IdentityRole<Guid>>(entity => { entity.ToTable("role"); });
		builder.Entity<IdentityUserRole<Guid>>(entity => { entity.ToTable("user_role"); });
		builder.Entity<IdentityUserClaim<Guid>>(entity => { entity.ToTable("user_claim"); });
		builder.Entity<IdentityUserLogin<Guid>>(entity => { entity.ToTable("user_login"); });
		builder.Entity<IdentityUserToken<Guid>>(entity => { entity.ToTable("user_token"); });
		builder.Entity<IdentityRoleClaim<Guid>>(entity => { entity.ToTable("role_claim"); });

		base.OnModelCreating(builder);

		// Apply a global filter for any entity with a Status property containing IsSoftDeleted
		foreach (var entityType in builder.Model.GetEntityTypes())
		{
			// Check if the entity has a Status property of type Status
			var statusProperty = entityType.FindProperty(nameof(Status));
			if (statusProperty != null && statusProperty.ClrType == typeof(Status))
			{
				builder.Entity(entityType.ClrType).HasQueryFilter(
					CreateIsSoftDeletedFilter(entityType.ClrType));
			}
		}


	
	}
	private static LambdaExpression CreateIsSoftDeletedFilter(Type entityType)
	{
		var parameter = Expression.Parameter(entityType, "entity");

		// Access the nested Status.IsSoftDeleted property
		var statusProperty =
			Expression.Property(parameter, nameof(Course.Status)); // Use one entity's property name as reference
		var isSoftDeletedProperty = Expression.Property(statusProperty, nameof(Status.IsSoftDeleted));

		// Create the condition Status.IsSoftDeleted == false
		var condition = Expression.Equal(isSoftDeletedProperty, Expression.Constant(false));

		return Expression.Lambda(condition, parameter);
	}
}




