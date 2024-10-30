using Application.Common.Operation;
using Application.DTOs.Identity;

namespace Application.Common.Interfaces
{
	public interface IRoleService
	{
		Task<OperationResult<string>> CreateRoleAsync(string roleName);
		Task<OperationResult> DeleteRoleAsync(string roleId);
		Task<OperationResult<List<RoleDto>>> GetRolesAsync();
		Task<OperationResult> UpdateRoleAsync(string id, string roleName);
		Task<OperationResult<RoleDto>> GetRoleByIdAsync(string roleId);
		// Additional methods for role-related operations...
	}

}
