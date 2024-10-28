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
		
}

