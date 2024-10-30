using Application.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Web.ProgramExtensions;

public static class AuthorizationServiceExtensions
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyName.CanManageUser, policy =>
                policy.Requirements.Add(new OwnerOrAdminRequirement()));
            options.AddPolicy(PolicyName.CanSeeUserPrivateInformation, policy =>
                policy.Requirements.Add(new OwnerOrAdminRequirement()));
        });
        services.AddSingleton<IAuthorizationHandler, OwnerOrAdminHandler>();

        return services;
    }
}