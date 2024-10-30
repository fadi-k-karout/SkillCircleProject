using Application.Common.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Web.Extensions;

namespace Web.Controllers.Identity
{


	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly SignInManager<User> _signInManager;
		private readonly IAuthenticationService _authenticationService;
		private readonly IConfiguration _configuration;

		public AuthController(SignInManager<User> signInManager, IAuthenticationService authenticationService, IConfiguration configuration, ILogger<UserController> logger)
		{
			_signInManager = signInManager;
			_authenticationService = authenticationService;
			_configuration = configuration;

		}


		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] (string email, string password) login)
		{
			var result = await _authenticationService.LoginAsync(login.email, login.password);
			return result.ToActionResult();
		}

		[AllowAnonymous]
		[HttpGet("external-login")]
		#region Swagger Documention and Reponse Types

		[SwaggerOperation(
			Summary = "Initiates external login with a specified provider.",
			Description = "Redirects the user to the external login provider" +
			              " (e.g., Google) for OAuth authentication."
		)]
		[SwaggerResponse(StatusCodes.Status302Found, "Redirects to external provider's login (Google OAuth, etc.).")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "If the request is invalid.")]

		#endregion
		public IActionResult ExternalLogin(string provider, string returnUrl)
		{
         			var baseUrl = $"{Request.Scheme}://{Request.Host}";
         			var redirectUrl = $"{baseUrl}{Url.Action(nameof(ExternalLoginCallback), "auth", new { returnUrl})}";
         			
         			var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
         			return Challenge(properties, provider);
		}

		[AllowAnonymous]
		[HttpGet("external-auth-callback")]
		#region Swagger Documention and Response Types
		[SwaggerOperation(
						Summary = "Handles the callback from the external provider after login.",
						 Description = "Processes the response from the external" +
						 "provider (Google OAuth, etc.) after the user has authenticated."
						)]
		[SwaggerResponse(StatusCodes.Status200OK, "User successfully authenticated and redirected.")]
		[SwaggerResponse(StatusCodes.Status400BadRequest, "If external login information is not available or invalid.")]
		#endregion
		public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
		{
			var info = await _signInManager.GetExternalLoginInfoAsync();

			if (info == null)
			{
				return BadRequest();
			}

			var result = await _authenticationService.ExternalLoginAsync(info);
			var userData = result.Value;
			var token = userData.Token;
			var userId = userData.UserId;
			var clientDomain = _configuration["Client:Domain"];
			
				// Set user data cookie
				var cookieOptions = new CookieOptions
				{
					Secure = true, // Only send over HTTPS
					SameSite = SameSiteMode.Lax,
					HttpOnly = false,
					Domain = clientDomain,
					Expires = DateTime.UtcNow.AddMinutes(10)
				};

				Response.Cookies.Append("token", token, cookieOptions);
				Response.Cookies.Append("userId", userId, cookieOptions);// Set user data cookie
			

			var returnUri = _configuration["Client:ReturnUrl"];
			return Redirect(returnUri);
		}

	}

}
