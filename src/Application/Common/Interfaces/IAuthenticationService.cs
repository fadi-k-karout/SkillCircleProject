using Microsoft.AspNetCore.Identity;
using Application.Common.Operation;
using Application.DTOs.Identity;

namespace Application.Common.Interfaces;

public interface IAuthenticationService
{
    Task<OperationResult<LoginResponseDto>> LoginAsync(string username, string password);
    Task<OperationResult<LoginResponseDto>> ExternalLoginAsync(ExternalLoginInfo info);
}