using Application.Common.Interfaces;
using Application.Common.Operation;
using Application.DTOs.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.Identity
{
	public class RoleService : IRoleService
	{
		private readonly RoleManager<IdentityRole<Guid>> _roleManager;

		public RoleService(RoleManager<IdentityRole<Guid>> roleManager)
		{
			_roleManager = roleManager;
		}


		public async Task<OperationResult<string>> CreateRoleAsync(string roleName)
		{
			
			var role = new IdentityRole<Guid>(roleName);
			var result = await _roleManager.CreateAsync(role);

			if (!result.Succeeded)
			{
				return OperationResult<string>.Failure(result.ToString());
			}

			return OperationResult<string>.Success(role.Id.ToString(), SuccessTypes.Created);
		}

		public async Task<OperationResult> DeleteRoleAsync(string roleId)
		{
			var role = await _roleManager.FindByIdAsync(roleId);
			if (role == null) return OperationResult.ResourceNotFound(nameof(role), roleId);

			var result = await _roleManager.DeleteAsync(role);
			if (!result.Succeeded)
			{
				return OperationResult.Failure(result.ToString());
			}

			return OperationResult.Success();
		}

		public async Task<OperationResult<List<RoleDto>>> GetRolesAsync()
		{
			try
			{
				// Fetch roles as entities first
				var roles =  _roleManager.Roles.ToList();

				// Map the roles to RoleDto
				var roleDtos = roles.Select(role => new RoleDto(role)).ToList();

				return OperationResult<List<RoleDto>>.Success(roleDtos);
			}
			catch (Exception ex)
			{

				return OperationResult<List<RoleDto>>.Failure(ex.Message);
			}
		}


		public async Task<OperationResult<RoleDto>> GetRoleByIdAsync(string roleId)
		{
			var role = await _roleManager.FindByIdAsync(roleId);

			if (role == null)
				return OperationResult<RoleDto>.ResourceNotFound(nameof(role), roleId);

			var roleDto = new RoleDto(role);
			return OperationResult<RoleDto>.Success(roleDto);
		}

		public async Task<OperationResult> UpdateRoleAsync(string roleId, string roleName)
		{
			var role = await _roleManager.FindByIdAsync(roleId);
			if (role == null) return OperationResult.ResourceNotFound(nameof(role), roleId);

			role.Name = roleName;
			var result = await _roleManager.UpdateAsync(role);

			if (!result.Succeeded)
			{
				return OperationResult.Failure(result.ToString());
			}

			return OperationResult.Success();
		}
	}

}
