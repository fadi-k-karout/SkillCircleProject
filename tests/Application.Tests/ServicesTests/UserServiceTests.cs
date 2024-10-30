using Application.Common.Operation;
using Application.DTOs.Identity;
using Application.Services.Identity;
using Domain.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.ServicesTests;

public class UserServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly UserService _userService;
    private readonly Mock<IMapper> _mapperMock; // Mock for IMapper

    public UserServiceTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null, null, null, null, null, null, null, null
        );

        // Setup the IMapper mock
        _mapperMock = new Mock<IMapper>();

        // Pass the mocked UserManager and IMapper to the UserService
        _userService = new UserService(_userManagerMock.Object, _mapperMock.Object /*, other dependencies */);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnSuccess_WhenUserIsCreated()
    {
        var dto = new CreateUserDto        
        { 
            UserName = "testuser",
            Email = "test@example.com", 
            FirstName = "Test", 
            Password = "Password123!" 
        };

        var user = new User(); // Create a new User instance
        
        // Setup the IMapper mock to map CreateUserDto to User
        _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);

        _userManagerMock.Setup(x => x.CreateAsync(user, dto.Password)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(x => x.AddToRoleAsync(user, "UserRole")).ReturnsAsync(IdentityResult.Success);

        var result = await _userService.CreateUserAsync(dto, "UserRole");

        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id.ToString(), result.Value); // Adjusting to get user ID after creation
    }

    [Fact]
    public async Task UpdateUserProfileAsync_ShouldReturnError_WhenUserNotFound()
    {
        var dto = new UpdateUserProfileDto { UserId = "1" };
        
        _userManagerMock.Setup(x => x.FindByIdAsync(dto.UserId)).ReturnsAsync((User)null);

        var result = await _userService.UpdateUserProfileAsync(dto);

        AssertError(result, "user with ID 1 was not found.", ErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateUserProfileAsync_ShouldReturnError_WhenUserNotActive()
    {
        var userId = Guid.NewGuid();
        var dto = new UpdateUserProfileDto { UserId = userId.ToString() };
        var user = new User { Id = userId , IsActive = false };
        
        _userManagerMock.Setup(x => x.FindByIdAsync(dto.UserId)).ReturnsAsync(user);

        var result = await _userService.UpdateUserProfileAsync(dto);

        AssertError(result, $"user with ID {userId.ToString()} was not found.", ErrorType.NotFound); // Assuming forbidden for inactive users
    }

    [Fact]
    public async Task UpdatePasswordAsync_ShouldReturnError_WhenUserNotFound()
    {
        var dto = new UpdatePasswordDto { UserId = "1", CurrentPassword = "OldPassword", NewPassword = "NewPassword123!" };
        
        _userManagerMock.Setup(x => x.FindByIdAsync(dto.UserId)).ReturnsAsync((User)null);

        var result = await _userService.UpdatePasswordAsync(dto);

        AssertError(result, "user with ID 1 was not found.", ErrorType.NotFound);
    }

    [Fact]
    public async Task UpdatePasswordAsync_ShouldReturnError_WhenUserNotActive()
    {
        var dto = new UpdatePasswordDto { UserId = "1", CurrentPassword = "OldPassword", NewPassword = "NewPassword123!" };
        var user = new User { Id = new Guid(), IsActive = false };
        
        _userManagerMock.Setup(x => x.FindByIdAsync(dto.UserId)).ReturnsAsync(user);

        var result = await _userService.UpdatePasswordAsync(dto);

        AssertError(result, "user with ID 1 was not found.", ErrorType.NotFound); // Assuming forbidden for inactive users
    }

    [Fact]
    public async Task GetUserDetailsAsync_ShouldReturnError_WhenUserNotFound()
    {
        var userId = "1";
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((User)null);
        
        var result = await _userService.GetUserDetailsAsync(userId);

        AssertError(result, "user with ID 1 was not found.", ErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturnError_WhenUserNotFound()
    {
        var userId = "1";
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((User)null);
        
        var result = await _userService.DeleteUserAsync(userId);

        AssertError(result, "user with ID 1 was not found.", ErrorType.NotFound);
    }



    [Fact]
    public async Task AssignUserToRoleAsync_ShouldReturnError_WhenUserNotFound()
    {
        var userId = "1";
        var roles = new List<string> { "Admin" };
        
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((User)null);
        
        var result = await _userService.AssignUserToRoleAsync(userId, roles);

        AssertError(result, "user with ID 1 was not found.", ErrorType.NotFound);
    }

    [Fact]
    public async Task DeactivateUserAsync_ShouldReturnError_WhenUserNotFound()
    {
        var userId = "1";
        _userManagerMock.Setup(x => x.FindByIdAsync(userId)).ReturnsAsync((User)null);
        
        var result = await _userService.DeactivateUserAsync(userId);

        AssertError(result, "user with ID 1 was not found.", ErrorType.NotFound);
    }

    [Fact]
    public async Task DeactivateUserAsync_ShouldReturnError_WhenUserNotActive()
    {
        var userId = new Guid();
        var user = new User { Id = userId, IsActive = false };
        
        _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        
        var result = await _userService.DeactivateUserAsync(userId.ToString());

        AssertError(result, "User is already inactive", ErrorType.BadRequest); // Assuming forbidden for already inactive users
    }

    [Fact]
    public async Task DeactivateUserAsync_ShouldReturnSuccess_WhenUserIsActive()
    {
        var userId = new Guid();
        var user = new User { Id = userId, IsActive = true };
        
        _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        var result = await _userService.DeactivateUserAsync(userId.ToString());

        Assert.True(result.IsSuccess);
        Assert.False(user.IsActive); // Check that the user is now inactive
    }

    private void AssertError(OperationResult result, string expectedErrorMessage, ErrorType expectedErrorType)
    {
        Assert.False(result.IsSuccess);
        Assert.Equal(expectedErrorMessage, result.ErrorMessage);
        Assert.Equal(expectedErrorType, result.ErrorType); // Assert the ErrorType as well
    }
}
