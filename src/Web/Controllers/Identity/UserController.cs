using System.ComponentModel.DataAnnotations;
using Application.Authorization;
using Application.Common.Interfaces;
using Application.DTOs.Identity;
using Application.Services.Identity;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Web.Extensions;
using Web.Templates;
namespace Web.Controllers.Identity
{
	[ApiController]
	[Route("api/users")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IAuthorizationService _authorizationService;
		private readonly UserManager<User> _userManager;
		private readonly EmailService _emailService;
		private readonly IConfiguration _configuration;
		private readonly RazorEmailRenderer _emailRenderer;
		
		private const string CanManageUser = PolicyName.CanManageUser;
		private const string CanSeeUserPrivateInformation = PolicyName.CanSeeUserPrivateInformation;
		private const string Admin = RoleName.Admin;

		public UserController(IUserService userService, IAuthorizationService authorizationService, UserManager<User> userManager, EmailService emailService, IConfiguration configuration, RazorEmailRenderer emailRenderer)
		{
			_userService = userService;
			_authorizationService = authorizationService;
			_userManager = userManager;
			_emailService = emailService;
			_configuration = configuration;
			_emailRenderer = emailRenderer;
		}
		
		[AllowAnonymous]
		[HttpPost("create-student")]
		#region Swagger Documentation and Response Types
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[SwaggerOperation(Summary = "Creates a new student user", Description = "Registers a new student.")]
		#endregion
		public async Task<IActionResult> CreateStudentAsync([FromBody] CreateUserDto dto)
		{
			var result = await _userService.CreateUserAsync(dto, RoleName.Student);
			if (result.IsSuccess)
			{
				var userId = result.Value;
				var user = await _userManager.FindByIdAsync(userId);
				if (user is null)
					throw new ApplicationException($"User with id {userId} could not be found.");
				
				await SendEmailConfirmation(user);
			}
			return result.ToActionResult();
		}

		[AllowAnonymous]
		[HttpPost("create-instructor")]
		#region Swagger Documentation and Response Types
         		[ProducesResponseType(StatusCodes.Status201Created)]
         		[ProducesResponseType(StatusCodes.Status400BadRequest)]
         		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
         		[SwaggerOperation(Summary = "Creates a new instructor user", Description = "Registers a new instructor.")]
         		#endregion
		public async Task<IActionResult> CreateInstructorAsync([FromBody] CreateUserDto dto)
		{
			var result = await _userService.CreateUserAsync(dto, RoleName.Creator);
			if (result.IsSuccess)
			{
				var userId = result.Value;
				var user = await _userManager.FindByIdAsync(userId);
				if (user is null)
					throw new ApplicationException($"User with id {userId} could not be found.");
				
				await SendEmailConfirmation(user);
				await SendWelcomeCreatorEmail(user);
			}
			
			return result.ToActionResult();
		}

		[Authorize(Roles=Admin)]
		[HttpPost("create-admin")]
		#region Swagger Documentation and Response Types
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[SwaggerOperation(Summary = "Creates a new Admin user")]
		#endregion
		public async Task<IActionResult> CreateAdminAsync([FromBody] CreateUserDto dto)
		{
			var result = await _userService.CreateUserAsync(dto, Admin);
			return result.ToActionResult();
		}
		
		[Authorize(Roles=Admin)]
		[HttpPost("create-moderator")]
		#region Swagger Documentation and Response Types
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[SwaggerOperation(Summary = "Creates a new moderator user")]
		#endregion
		public async Task<IActionResult> CreateModeratorAsync([FromBody] CreateUserDto dto)
		{
			var result = await _userService.CreateUserAsync(dto, "moderator");
			return result.ToActionResult();
		}
		
		[Authorize(Policy = CanManageUser)]
		[HttpPut("update")]
		#region Swagger Documentation and Response Types
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[SwaggerOperation(Summary = "Updates user profile", Description = "Updates the profile of the specified user.")]
		#endregion
		public async Task<IActionResult> UpdateUserProfileAsync([FromBody] UpdateUserProfileDto dto)
		{
			var result = await _userService.UpdateUserProfileAsync(dto);
			return await result.ToActionResultBuilder(_authorizationService, User)
				         .WithAuthorizationAsync(dto.UserId, CanManageUser);
		}
		
		[Authorize]

		#region Swagger Documentation and Response Types
		[HttpPost("{userId}/upload-profile-photo")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[SwaggerOperation(Summary = "Uploads a profile photo for the user by user ID.")]
		#endregion
		public async Task<IActionResult> UploadProfilePhoto(string userId, IFormFile file,CancellationToken ct)
		{
			// Validate the userId
			if (string.IsNullOrWhiteSpace(userId))
				return BadRequest("Invalid user ID.");

			var user = await _userManager.FindByIdAsync(userId);
			if (user is null)
				return NotFound("User not found.");

			// Rest of the file validation and processing logic remains the same
			if (file == null || file.Length == 0)
				return BadRequest("No file uploaded.");

			if (file.Length > 10 * 1024 * 1024) // 10 MB limit
				return BadRequest("File size exceeds 10 MB.");

			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
			var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
			if (!allowedExtensions.Contains(extension))
				return BadRequest("Invalid file type. Only JPG, PNG, and GIF files are allowed.");

			var fileName = $"{Guid.NewGuid()}{extension}";
			var uploadsFolder = _configuration["Server:UploadsFolder"] ?? "/uploads";
			var filePath = Path.Combine(uploadsFolder, fileName);

			// Create the file stream with cancellation token
			await using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
			{
				// Use the cancellation token here
				await file.CopyToAsync(stream, ct);
			}

			var baseUrl = _configuration["Server:BaseUrl"]; // Your backend domain, e.g., "https://api.yourdomain.com"
			var photoUrl = $"{baseUrl}/{filePath}";

			user.PhotoUrl = photoUrl; // Assuming you want to save the full URL
			var updateResult = await _userManager.UpdateAsync(user);

			if (!updateResult.Succeeded)
				return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred, please try again later.");

			// Return the file name or URL
			return Ok(new { fileName, photoUrl });
		}
		
		[Authorize(Policy = CanManageUser)]
		[HttpPost("change-password")]
		#region Swagger Documentation and Response Types
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[SwaggerOperation(Summary = "Changes the password", Description = "Allows the user to change their password.")]
		#endregion
		public async Task<IActionResult> ChangePasswordAsync([FromBody] UpdatePasswordDto dto)
		{
			var result = await _userService.UpdatePasswordAsync(dto);
			return await result.ToActionResultBuilder(_authorizationService, User)
				         .WithAuthorizationAsync(dto.UserId, CanManageUser);
		}
		
		[HttpPost("generate-password-reset-token")]
		#region Swagger Documentation and Response Types
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[SwaggerOperation(Summary = "Generates a password reset token", Description = "Generates a password reset token for the user and sends it via email.")]
		#endregion
		public async Task<IActionResult> GeneratePasswordResetTokenAsync([FromBody] string email)
		{
			if (string.IsNullOrEmpty(email) || !new EmailAddressAttribute().IsValid(email))
				return BadRequest("Invalid email address.");
			
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
    
			// Assuming the reset link URL is defined in your configuration
			var clientUrl = _configuration["ClientApp:PasswordResetUrl"];
			var resetLink = $"{clientUrl}?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(user.Email)}";
			
			// Prepare the email model
			var model = new PasswordResetEmailModel
			{
				UserName = user.FirstName + user.LastName,
				ResetLink = resetLink,
				Subject = "Password Reset Request"
			};

			// Render the Razor view as an HTML string
			var htmlContent = await _emailRenderer.RenderTemplateAsync("PasswordResetEmail", model);

			// Send the email using the application layer's service
			await _emailService.SendEmailAsync(email, model.Subject, htmlContent);

		
			return Ok("Password reset link has been sent to your email.");
		}
		
		[HttpPost("reset-password")]
		#region Swagger Documentation and Response Types
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[SwaggerOperation(Summary = "Resets the user password", 
			Description = "Resets the password using a reset token.")]
		#endregion
		public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDto resetPasswordDto)
		{
			var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
			if (user == null)
			{
				return NotFound("User not found.");
			}

			var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}

			return Ok("Password has been reset successfully.");
		}
		
		[HttpPost("send-email-verification")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[SwaggerOperation(Summary = "Send Email Verification", Description = "Send an email verification link to the user.")]
		public async Task<IActionResult> SendEmailVerification([FromBody] string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			
			if (user == null)
			{
				return NotFound("User not found.");
			}
			
			await SendEmailConfirmation(user);

			return Ok("Email verification link has been sent.");
		}

		private async Task SendEmailConfirmation(User user)
		{
			var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var clientUrl = _configuration["ClientApp:EmailVerificationUrl"];
			var verificationLink = $"{clientUrl}?token={Uri.EscapeDataString(verificationToken)}&email={Uri.EscapeDataString(user.Email)}";

			var model = new EmailVerificationEmailModel
			{
				UserName = user.FirstName + user.LastName,
				VerificationLink = verificationLink
			};
			var htmlContent = await _emailRenderer.RenderTemplateAsync("EmailVerificationEmail", model);

			await _emailService.SendEmailAsync(user.Email, model.Subject, htmlContent);

		}
		private async Task SendWelcomeCreatorEmail(User user)
		{
			
			var model = new WelcomeEmailModel()
			{
				FirstName = user.FirstName ,
				LastName = user.LastName,
				Subject = "Welcome To Skill Circle Project",
				ContactEmail = "support@skillcircleproject.com",
			};
			var htmlContent = await _emailRenderer.RenderTemplateAsync("WelcomeEmail", model);

			await _emailService.SendEmailAsync(user.Email, model.Subject, htmlContent);

		}
		
		
        [Authorize]
		[HttpGet("{userId}")]
		#region Swagger Documentation and Response Types 
		[ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[SwaggerOperation(Summary = "Gets user details", Description = "Retrieves details of the user by ID.")]
		#endregion
		public async Task<IActionResult> GetUserDetailsAsync(string userId)
		{
			var result = await _userService.GetUserDetailsAsync(userId);
			var requirement = new OwnerOrAdminRequirement();
				
			return await result.ToActionResultBuilder(_authorizationService, User)
				.WithAuthorizationAsync(userId, CanSeeUserPrivateInformation);
		}
		
		[Authorize(Roles=Admin)]
		[HttpGet("paginated")]
		public async Task<IActionResult> GetUsersAsync(int pageNumber, int pageSize)
		{
			var result = await _userService.GetUsersAsync(pageNumber, pageSize);
			return result.ToActionResult();
		}

		[Authorize(Roles=Admin)]
		[HttpGet]
		#region Swagger Documentation and Respone Types
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		#endregion
		public async Task<IActionResult> GetUsersCountAsync()
		{
			var userCount = await _userService.GetTotalUsersCountAsync();
			return Ok(userCount);
		}
		
		[Authorize(Policy = CanManageUser)]
		[HttpPut("{userId}/deactivate")]
		public async Task<IActionResult> DeactivateUserAsync(string userId)
		{
			var result = await _userService.DeactivateUserAsync(userId);
			return await result.ToActionResultBuilder(_authorizationService, User)
				.WithAuthorizationAsync(userId, CanManageUser);
		}
		
		[Authorize(Policy = CanManageUser)]
		[HttpPut("{userId}/activate")]
		public async Task<IActionResult> ActivateUserAsync(string userId)
		{
			var result = await _userService.ActivateUserAsync(userId);
			return await result.ToActionResultBuilder(_authorizationService, User)
				.WithAuthorizationAsync(userId, CanManageUser);
		}
		
		[Authorize(Roles = Admin)]
		[HttpDelete("{userId}")]
		public async Task<IActionResult> DeleteUserAsync(string userId)
		{
			var result = await _userService.DeleteUserAsync(userId);
			return result.ToActionResult();
		}

		[Authorize(Roles = Admin)]
		[HttpPost("{userId}/roles")]
		public async Task<IActionResult> AssignUserToRoleAsync([FromBody](string userId, IList<string> roles) role)
		{
			var result = await _userService.AssignUserToRoleAsync(role.userId, role.roles);
			return result.ToActionResult();
		}
		
		[Authorize(Roles = Admin)]
		[HttpPut("{userId}/roles")]
		public async Task<IActionResult> UpdateUserRolesAsync([FromBody](string userId, IList<string> newRoles) updateRole)
		{
			var result = await _userService.UpdateUserRolesAsync(updateRole.userId, updateRole.newRoles);
			return result.ToActionResult();

		}
		
		[Authorize(Roles = Admin)]
		[HttpDelete("{userId}/roles")]
		public async Task<IActionResult> RemoveUserFromRoleAsync([FromBody](string userId, string newRoles) updateRole)
		{
			var result = await _userService.RemoveUserFromRoleAsync(updateRole.userId, updateRole.newRoles);
			return result.ToActionResult();

		}



	}

}
