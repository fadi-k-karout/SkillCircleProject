using Application.Common.Interfaces;
using Application.Common.Operation;
using Application.DTOs.Identity;
using Domain.Models;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Identity
{


	public class UserService : IUserService
	{
		private readonly UserManager<User> _userManager;
		private readonly IMapper _mapper;
		private readonly string userNotFound = "User not found.";
		

		public UserService(UserManager<User> userManager, IMapper mapper)
		{
			_userManager = userManager;
			_mapper = mapper;
		}

		public async Task<OperationResult<string>> CreateUserAsync(CreateUserDto dto, string role)
		{

			if(dto == null || role.IsNullOrEmpty())
			{
				return OperationResult<string>.ArgumentNullError($"{nameof(User)} or {nameof(role)}");
			}
			var user = _mapper.Map<User>(dto);

            user.Id = Guid.NewGuid();
			var result = await _userManager.CreateAsync(user, dto.Password);
			if (!result.Succeeded)
			{
				return OperationResult<string>.Failure(result.ToString());
			}

			var addToRolesResult = await _userManager.AddToRoleAsync(user, role);
			if (!addToRolesResult.Succeeded)
			{
				return OperationResult<string>.Failure(addToRolesResult.ToString());
			}

			return OperationResult<string>.Success(user.Id.ToString(), SuccessTypes.Created);
		}

		public async Task<OperationResult> UpdateUserProfileAsync(UpdateUserProfileDto dto)
		{
			var userId = dto.UserId;
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null || user.IsActive == false)
			{
				return OperationResult.ResourceNotFound(nameof(user), userId);
			}

			var updatedUser = dto.Adapt(user); // This will only map non-null properties from dto to user

			var updateResult = await _userManager.UpdateAsync(updatedUser);
			if (!updateResult.Succeeded)
			{
				return OperationResult.Failure(updateResult.ToString());
			}

			return OperationResult.Success();
		}

		public async Task<OperationResult> UpdatePasswordAsync(UpdatePasswordDto dto)
		{
			var userId = dto.UserId;
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null || user.IsActive == false)
			{
				return OperationResult.ResourceNotFound(nameof(user), userId);
			}

			var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
			if (!result.Succeeded)
			{
				return OperationResult.Failure(result.ToString());
			}

			return OperationResult.Success();
		}

		public async Task<OperationResult<UserDetailsDto>> GetUserDetailsAsync(string userId)
		{
			
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null || user.IsActive == false)
			{
				return OperationResult<UserDetailsDto>.ResourceNotFound(nameof(user), userId);
			}

			//var roles = await _userManager.GetRolesAsync(user);
			var userDetails = new UserDetailsDto(user); // Using the new constructor

			return OperationResult<UserDetailsDto>.Success(userDetails);
		}

		public async Task<OperationResult<List<UserDetailsDto>>> GetUsersAsync(int pageNumber, int pageSize)
		{
			try
			{
				// Fetch paginated users
				var users = await _userManager.Users
					.Skip((pageNumber - 1) * pageSize)
					.Take(pageSize)
					.ToListAsync();

				
				var userDtos = new List<UserDetailsDto>();
				foreach (var user in users)
				{
					
					userDtos.Add(new UserDetailsDto(user));
				}

				
				return OperationResult<List<UserDetailsDto>>.Success(userDtos);
			}
			catch (Exception ex)
			{
				
				return OperationResult<List<UserDetailsDto>>.Failure($"An error occurred while fetching users: {ex.Message}");
			}
		}

		public async Task<int> GetTotalUsersCountAsync()
		{
			return await _userManager.Users.CountAsync();
		}


		public async Task<OperationResult> UpdateUserRolesAsync(string userId, IList<string> newRoles)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null || user.IsActive == false)
		        return OperationResult.ResourceNotFound(nameof(user), userId);

			var existingRoles = await _userManager.GetRolesAsync(user);
			var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
			if (!removeRolesResult.Succeeded)
			{
				return OperationResult.Failure(removeRolesResult.ToString());
			}

			var addRolesResult = await _userManager.AddToRolesAsync(user, newRoles);
			if (!addRolesResult.Succeeded)
			{
				return OperationResult.Failure(addRolesResult.ToString());
			}

			return OperationResult.Success();
		}
		public async Task<OperationResult> RemoveUserFromRoleAsync(string userId,string  role)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null || user.IsActive == false)
				return OperationResult.ResourceNotFound(nameof(user), userId);

			var existingRoles = await _userManager.GetRolesAsync(user);
			if (!existingRoles.Contains(role))
				return OperationResult.Failure("The User is not in the role.", ErrorType.BadRequest);
			
			var removeRolesResult = await _userManager.RemoveFromRoleAsync(user, role);
			
			if (!removeRolesResult.Succeeded)
			{
				return OperationResult.Failure(removeRolesResult.ToString());
			}
			

			return OperationResult.Success();
		}
		public async Task<OperationResult> AssignUserToRoleAsync(string userId, IList<string> roles)
		{
		
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null || user.IsActive == false)
			{
				return OperationResult.ResourceNotFound(nameof(user), userId);
			}

			try
			{
			
				var currentRoles = await _userManager.GetRolesAsync(user);
				var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
				if (!removeResult.Succeeded)
				{
					var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
					return OperationResult.Failure($"Failed to remove user from current roles: {errors}");
				}

			
				var addResult = await _userManager.AddToRolesAsync(user, roles);
				if (!addResult.Succeeded)
				{
					
					return OperationResult.Failure($"Failed to assign user to roles: {addResult.ToString()}");
				}

				
				return OperationResult.Success();
			}
			catch (Exception ex)
			{
				return OperationResult.Failure($"An error occurred while assigning roles: {ex.Message}");
			}
		}



		public async Task<OperationResult> DeleteUserAsync(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return OperationResult.ResourceNotFound(nameof(user), userId);
			}

			var result = await _userManager.DeleteAsync(user);
			if (!result.Succeeded)
			{
				return OperationResult.Failure(result.ToString());
			}

			return OperationResult.Success();
		}

		public async Task<OperationResult> DeactivateUserAsync(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return OperationResult.ResourceNotFound(nameof(user), userId);
			}
			if (!user.IsActive) return OperationResult.Failure("User is already inactive", ErrorType.BadRequest);

			user.IsActive = false; // Deactivate the user
			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				return OperationResult.Failure(result.ToString());
			}

			return OperationResult.Success();
		}

		public async Task<OperationResult> ActivateUserAsync(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return OperationResult.ResourceNotFound(nameof(user), userId);
			}

			if (user.IsActive) return OperationResult.Failure("User is already active", ErrorType.BadRequest);
			
			user.IsActive = true; // Activate the user
			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				return OperationResult.Failure(result.ToString());
			}

			return OperationResult.Success();
		}
	}



}