using Application.Authorization;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Web.Extensions;

namespace Web.Controllers.Identity
{
	[Authorize(Roles = RoleName.Admin)]
	[ApiController]
	[Route("api/roles")]
	public class RoleController : ControllerBase
	{
		private readonly IRoleService _roleService;

		public RoleController(IRoleService roleService)
		{
			_roleService = roleService;
		}

		[HttpPost("create")]
		#region Swagger Documentation and Response Types
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[SwaggerOperation(Summary = "Creates a new role", Description = "Creates a new role with the specified name.")]
		#endregion
		public async Task<IActionResult> CreateRoleAsync([FromBody] string roleName)
		{
			var result = await _roleService.CreateRoleAsync(roleName);
			return result.ToActionResult();
		}

		[HttpDelete("{roleId}")]
		#region Swagger Documentation and Response Types
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[SwaggerOperation(Summary = "Deletes a role", Description = "Deletes the role with the specified ID.")]
		#endregion
		public async Task<IActionResult> DeleteRoleAsync(string roleId)
		{
			var result = await _roleService.DeleteRoleAsync(roleId);
			return result.ToActionResult();
		}

		[HttpGet]
		#region Swagger Documentation and Response Types
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[SwaggerOperation(Summary = "Gets all roles", Description = "Retrieves a list of all roles.")]
		#endregion
		public async Task<IActionResult> GetRolesAsync()
		{
			var result = await _roleService.GetRolesAsync();
			return result.ToActionResult();
		}

		[HttpGet("{roleId}")]
		#region Swagger Documentation and Response Types
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[SwaggerOperation(Summary = "Gets role by ID", Description = "Retrieves the details of the role with the specified ID.")]
		#endregion
		public async Task<IActionResult> GetRoleByIdAsync(string roleId)
		{
			var result = await _roleService.GetRoleByIdAsync(roleId);
			return result.ToActionResult();
		}

		[HttpPut("{roleId}")]
		#region Swagger Documentation and Response Types
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[SwaggerOperation(Summary = "Updates a role", Description = "Updates the name of the role with the specified ID.")]
		#endregion
		public async Task<IActionResult> UpdateRoleAsync(string roleId, [FromBody] string roleName)
		{
			var result = await _roleService.UpdateRoleAsync(roleId, roleName);
			return result.ToActionResult();
		}
	}


}
