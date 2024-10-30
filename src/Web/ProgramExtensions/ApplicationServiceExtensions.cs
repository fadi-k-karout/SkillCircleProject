using Application.Common.Interfaces;
using Application.Services.Identity;
using Mapster;
using MapsterMapper;


namespace Web.ProgramExtensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(AppDomain.CurrentDomain.GetAssemblies()); // optional if you have custom mappings

        services.AddSingleton(config);
        services.AddScoped<IMapper, Mapper>();
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();

        return services;
    }
}