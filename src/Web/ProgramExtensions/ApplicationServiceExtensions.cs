using Application.Common.Interfaces;
using Application.Common.Interfaces.Content;
using Application.Common.Interfaces.repos;
using Application.Services.Content;
using Application.Services.Identity;
using Infrastructure.Repositories;
using Infrastructure.UnitOfWork;
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
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        

        #region Identity
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        #endregion

        #region Content
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ISkillService, SkillService>();
        services.AddScoped<IVideoService, VideoService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ICdnService, ApiVideoService>();
        
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<IVideoRepository, VideoRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        #endregion
        
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        
        

        return services;
    }
}