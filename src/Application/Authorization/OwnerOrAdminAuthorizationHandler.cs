using Microsoft.AspNetCore.Authorization;

namespace Application.Authorization;

public class OwnerOrAdminHandler : AuthorizationHandler<OwnerOrAdminRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnerOrAdminRequirement requirement)
    {
        var userId = context.User.FindFirst("sub")?.Value;
        var resourceOwnerId =  context.Resource as string; 

        if (context.User.IsInRole("Admin") || userId == resourceOwnerId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
