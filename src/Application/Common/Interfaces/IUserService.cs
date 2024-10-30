using Application.Common.Operation;
using Application.DTOs.Identity;

namespace Application.Common.Interfaces
{
	public interface IUserService
	{
		Task<OperationResult<string>> CreateUserAsync(CreateUserDto dto, string role);
		Task<OperationResult> UpdateUserProfileAsync(UpdateUserProfileDto dto);
		Task<OperationResult> UpdatePasswordAsync(UpdatePasswordDto dto);
		Task<OperationResult> AssignUserToRoleAsync(string userId, IList<string> roles);
		Task<OperationResult> UpdateUserRolesAsync(string userId, IList<string> newRoles);
		Task<OperationResult> RemoveUserFromRoleAsync(string userId, string role);

		Task<OperationResult<UserDetailsDto>> GetUserDetailsAsync(string userId);
		Task<OperationResult<List<UserDetailsDto>>> GetUsersAsync(int pageNumber, int pageSize);
		Task<int> GetTotalUsersCountAsync();

		Task<OperationResult> DeactivateUserAsync(string userId);
		Task<OperationResult> ActivateUserAsync(string userId);
		Task<OperationResult> DeleteUserAsync(string userId);
	}

}
