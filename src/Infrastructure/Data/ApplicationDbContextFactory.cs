using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
	public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
	{
		public ApplicationDbContext CreateDbContext(string[] args)
		{
			var currentDirectory = Directory.GetCurrentDirectory();
			var solutionDir = Directory.GetParent(currentDirectory)?.FullName; 

			var configPath = Path.Combine(solutionDir, "Web", "appsettings.Development.json");


		
			var configuration = new ConfigurationBuilder()
				.SetBasePath(solutionDir) 
				.AddJsonFile(configPath, optional: false, reloadOnChange: true)
				.Build();

		
			var connectionString = configuration.GetConnectionString("DefaultConnection");

			var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
			optionsBuilder.UseSqlServer(connectionString);

			return new ApplicationDbContext(optionsBuilder.Options);
		}
	}
}