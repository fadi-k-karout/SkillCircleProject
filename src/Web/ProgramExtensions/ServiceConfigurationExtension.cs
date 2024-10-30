using Microsoft.AspNetCore.Mvc;

namespace Web.ProgramExtensions;

public static class ServiceConfigurationExtension
{
    public static IServiceCollection AddCustomValidationApiBehaviour(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                // Extract errors from ModelState with property names
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,  // Property name
                        e => e.Value.Errors.Select(x => x.ErrorMessage).ToArray()  // Error messages
                    );

                // Return a 400 response with the validation errors mapped to properties
                return new BadRequestObjectResult(new { errors });
            };
        });
        return services;
    }
    
}