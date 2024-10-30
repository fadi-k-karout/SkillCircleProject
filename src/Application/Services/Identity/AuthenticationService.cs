using System.Security.Claims;
using Application.Common.Interfaces;
using Application.Common.Operation;
using Application.DTOs.Identity;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Identity
{
   public class AuthenticationService : IAuthenticationService
	{
		private readonly UserManager<User> _userManager;
		private readonly ITokenGenerator _tokenGenerator;

		public AuthenticationService(UserManager<User> userManager, ITokenGenerator tokenGenerator)
		{
			_userManager = userManager;
		
			_tokenGenerator = tokenGenerator;
		}

		public async Task<OperationResult<LoginResponseDto>> LoginAsync(string email, string password)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user is null)
				return OperationResult<LoginResponseDto>.AuthenticationError();
			
			var passwordValid = await _userManager.CheckPasswordAsync(user, password);
			if(!passwordValid)
				return OperationResult<LoginResponseDto>.AuthenticationError();
			
			var error = await PreSignInCheck(user);
			if (error is not null)
				return OperationResult<LoginResponseDto>.Failure(error, ErrorType.BadRequest);
			
			

			var userId = user.Id.ToString();
			// Generate JWT token
			var roles = await _userManager.GetRolesAsync(user);
			var token = _tokenGenerator.GenerateToken(userId, user.UserName, roles.ToList());
			
			var dto = new LoginResponseDto
			{
				UserId = userId,
				Token = token,

			};

			return OperationResult<LoginResponseDto>.Success(dto);
		}

		public async Task<OperationResult<LoginResponseDto>> ExternalLoginAsync(ExternalLoginInfo info)
		{
			var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
			if (user is null)
			{
				// Create user if it doesn't exist
				var email = info.Principal?.FindFirstValue(ClaimTypes.Email);
				var firstName = info.Principal?.FindFirstValue(ClaimTypes.GivenName);
				if (email.IsNullOrEmpty() || firstName.IsNullOrEmpty())
					return OperationResult<LoginResponseDto>
						   .Failure("External login failure, missing required claims");
				
				user = new User
				{
					Id = Guid.NewGuid(),
					UserName = email,
					Email = email,
					FirstName = firstName,
					LastName = info.Principal?.FindFirstValue(ClaimTypes.Surname) 
				};

				var creationResult = await _userManager.CreateAsync(user);
				if (!creationResult.Succeeded)
				{
					return OperationResult<LoginResponseDto>.Failure("Failed to create user.");
				}
				var addLoginAsync = _userManager.AddLoginAsync(user, info);

			}

			 
			 // Generate JWT token
	
			 var roles = await _userManager.GetRolesAsync(user) ?? new List<string>(); // Ensure roles is not null;
		     var token = _tokenGenerator.GenerateToken(user.Id.ToString(), user.UserName, roles.ToList());
		     var dto = new LoginResponseDto
		     {
			     UserId = user.Id.ToString(),
			     Token = token,

		     };
			 return OperationResult<LoginResponseDto>.Success(dto);

		}


		private async Task<string?> PreSignInCheck(User user)
		{
			// Check if the user is locked out
			if (await _userManager.IsLockedOutAsync(user))
			{
				return "User is locked out.";
			}

			// Check if the user requires two-factor authentication
			if (await _userManager.GetTwoFactorEnabledAsync(user))
			{
				return "User requires two-factor authentication.";
			}

			// Check if the user needs to confirm their email
			if (!_userManager.IsEmailConfirmedAsync(user).Result)
			{
				return "Email not confirmed.";
			}

			// Additional checks can go here (e.g., account disabled, expired credentials, etc.)
			return null; // Return null if all checks pass
		}

	}
}
