using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Web.ProgramExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }

}