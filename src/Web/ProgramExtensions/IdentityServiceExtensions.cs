using System.Net.Http.Headers;
using System.Text;
using Application.Services.Identity;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace Web.ProgramExtensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddCustomIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole<Guid>>()
            .AddRoles<IdentityRole<Guid>>()
            .AddSignInManager<SignInManager<User>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        
        
        services.AddHttpClient();



        // Register EmailService and IConfiguration
        services.AddTransient<EmailService>();

     
        return services;
    }
}