using System.Security.Claims;
using Application.Common.Operation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Extensions;

public class ActionResultBuilder<T>
{
    private readonly OperationResult<T> _result;
    private readonly IAuthorizationService _authorizationService;
    private readonly ClaimsPrincipal _user;

    public ActionResultBuilder(OperationResult<T> result, IAuthorizationService authorizationService, ClaimsPrincipal user)
    {
        _result = result;
        _authorizationService = authorizationService;
        _user = user;
    }
    

    public async Task<IActionResult> WithAuthorizationAsync(object resource, string policy)
    {
        // Authorization logic
        var authorizationResult = await _authorizationService.AuthorizeAsync(_user, resource, policy);
        if (!authorizationResult.Succeeded)
        {
            return new ForbidResult();
        }

        // Convert the result to IActionResult
        return _result.ToActionResult();
    }
}

public class ActionResultBuilder
{
    private readonly OperationResult _result;
    private readonly IAuthorizationService _authorizationService;
    private readonly ClaimsPrincipal _user;

    public ActionResultBuilder(OperationResult result, IAuthorizationService authorizationService, ClaimsPrincipal user)
    {
        _result = result;
        _authorizationService = authorizationService;
        _user = user;
    }
    

    public async Task<IActionResult> WithAuthorizationAsync(object resource, string policy)
    {
        // Authorization logic
        var authorizationResult = await _authorizationService.AuthorizeAsync(_user, resource, policy);
        if (!authorizationResult.Succeeded)
        {
            return new ForbidResult();
        }

        // Convert the result to IActionResult
        return _result.ToActionResult();
    }
}
