using Application.Authorization;
using Application.Common.Interfaces;
using Application.DTOs.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Web.Extensions;

namespace Web.Controllers.Identity
{
	[ApiController]
	[Route("api/users")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly IAuthorizationService _authorizationService;
		
		private const string CanManageUser = PolicyName.CanManageUser;
		private const string CanSeeUserPrivateInformation = PolicyName.CanSeeUserPrivateInformation;
		private const string Admin = RoleName.Admin;

		public UserController(IUserService userService, IAuthorizationService authorizationService)
		{
			_userService = userService;
			_authorizationService = authorizationService;
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
			var result = await _userService.CreateUserAsync(dto, "Student");
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
			var result = await _userService.CreateUserAsync(dto, "Instructor");
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
				.WithAuthorizationAsync(userId, CanSeeUserPrivateInformation);
		}
		
		[Authorize(Policy = CanManageUser)]
		[HttpPut("{userId}/activate")]
		public async Task<IActionResult> ActivateUserAsync(string userId)
		{
			var result = await _userService.ActivateUserAsync(userId);
			return await result.ToActionResultBuilder(_authorizationService, User)
				.WithAuthorizationAsync(userId, CanSeeUserPrivateInformation);
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
