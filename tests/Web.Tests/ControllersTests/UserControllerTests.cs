using System.Security.Claims;
using Application.Common.Interfaces;
using Application.Common.Operation;
using Application.DTOs.Identity;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Controllers.Identity;

namespace Web.Tests.ControllersTests;

public class UserControllerTests
{
    private readonly UserController _controller;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IAuthorizationService> _authorizationServiceMock;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _controller = new UserController(_userServiceMock.Object, _authorizationServiceMock.Object);
    }

    [Fact]
    public async Task CreateStudentAsync_ShouldReturn201Created_WhenUserIsCreatedSuccessfully()
    {
        // Arrange
        var dto = new CreateUserDto { UserName = "testuser", Password = "Password123", Email = "test@example.com", FirstName = "Test" };
        var operationResult = new OperationResult<string> { IsSuccess = true,  SuccessType = SuccessTypes.Created};
        _userServiceMock.Setup(service => service.CreateUserAsync(dto, "Student"))
            .ReturnsAsync(operationResult);

        // Act
        var result = await _controller.CreateStudentAsync(dto);

        // Assert
        result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task CreateStudentAsync_ShouldReturn400BadRequest_WhenUserCreationFails()
    {
        // Arrange
        var dto = new CreateUserDto { UserName = "testuser", Password = "Password123", Email = "test@example.com", FirstName = "Test" };
        var operationResult = new OperationResult<string>
        {
            IsSuccess = false,
            ErrorType = ErrorType.BadRequest,
            ErrorMessage = "Bad Request Error"
        };
        _userServiceMock.Setup(service => service.CreateUserAsync(dto, "Student"))
            .ReturnsAsync(operationResult);

        // Act
        var result = await _controller.CreateStudentAsync(dto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetUserDetailsAsync_ShouldReturn200Ok_WhenUserIsFound()
    {
        // Arrange
        string userId = "some-user-id";
        var userDetailsDto = new UserDetailsDto(new User { Id = Guid.NewGuid(), FirstName = "Test", UserName = "testuser", Email = "test@example.com" });
        var operationResult = new OperationResult<UserDetailsDto>
        {
            IsSuccess = true,
            Value = userDetailsDto
        };
        _userServiceMock.Setup(service => service.GetUserDetailsAsync(userId))
            .ReturnsAsync(operationResult);
        _authorizationServiceMock.Setup(auth => auth.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(AuthorizationResult.Success());

        // Act
        var result = await _controller.GetUserDetailsAsync(userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(userDetailsDto);
    }

    [Fact]
    public async Task GetUserDetailsAsync_ShouldReturn404NotFound_WhenUserIsNotFound()
    {
        // Arrange
        string userId = "some-user-id";
        var operationResult = new OperationResult<UserDetailsDto>
        {
            IsSuccess = false,
            ErrorType = ErrorType.NotFound,
            ErrorMessage = "User not found"
        };
        _userServiceMock.Setup(service => service.GetUserDetailsAsync(userId))
            .ReturnsAsync(operationResult);
        _authorizationServiceMock.Setup(auth => auth.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(AuthorizationResult.Success());

        // Act
        var result = await _controller.GetUserDetailsAsync(userId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateUserProfileAsync_ShouldReturn204NoContent_WhenUserProfileIsUpdatedSuccessfully()
    {
        // Arrange
        var dto = new UpdateUserProfileDto { UserId = "some-user-id", FirstName = "Updated" };
        var operationResult = new OperationResult { IsSuccess = true };
        _userServiceMock.Setup(service => service.UpdateUserProfileAsync(dto))
            .ReturnsAsync(operationResult);
        _authorizationServiceMock.Setup(auth => auth.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(AuthorizationResult.Success());

        // Act
        var result = await _controller.UpdateUserProfileAsync(dto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task UpdateUserProfileAsync_ShouldReturn400BadRequest_WhenUpdateFails()
    {
        // Arrange
        var dto = new UpdateUserProfileDto { UserId = "some-user-id", FirstName = "Updated" };
        var operationResult = new OperationResult
        {
            IsSuccess = false,
            ErrorType = ErrorType.BadRequest,
            ErrorMessage = "Update failed"
        };
        _userServiceMock.Setup(service => service.UpdateUserProfileAsync(dto))
            .ReturnsAsync(operationResult);
        _authorizationServiceMock.Setup(auth => auth.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(AuthorizationResult.Success());

        // Act
        var result = await _controller.UpdateUserProfileAsync(dto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturn204NoContent_WhenPasswordIsChangedSuccessfully()
    {
        // Arrange
        var dto = new UpdatePasswordDto { UserId = "some-user-id", CurrentPassword = "OldPassword", NewPassword = "NewPassword123" };
        var operationResult = new OperationResult { IsSuccess = true };
        _userServiceMock.Setup(service => service.UpdatePasswordAsync(dto))
            .ReturnsAsync(operationResult);
        // Mock WithAuthorizationAsync if necessary to return a successful authorization
        _authorizationServiceMock.Setup(auth => auth.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(AuthorizationResult.Success());

        // Act
        var result = await _controller.ChangePasswordAsync(dto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldReturn400BadRequest_WhenChangePasswordFails()
    {
        // Arrange
        var dto = new UpdatePasswordDto { UserId = "some-user-id", CurrentPassword = "OldPassword", NewPassword = "NewPassword123" };
        var operationResult = new OperationResult
        {
            IsSuccess = false,
            ErrorType = ErrorType.BadRequest,
            ErrorMessage = "Change password failed"
        };
        _userServiceMock.Setup(service => service.UpdatePasswordAsync(dto))
            .ReturnsAsync(operationResult);
        _authorizationServiceMock.Setup(auth => auth.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(AuthorizationResult.Success());

        // Act
        var result = await _controller.ChangePasswordAsync(dto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetUsersCountAsync_ShouldReturn200Ok_WhenUsersCountIsRetrieved()
    {
        // Arrange
        int userCount = 10;
        _userServiceMock.Setup(service => service.GetTotalUsersCountAsync())
            .ReturnsAsync(userCount);

        // Act
        var result = await _controller.GetUsersCountAsync();

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(userCount);
    }

    [Fact]
    public async Task GetUsersAsync_ShouldReturn200Ok_WhenUsersAreRetrieved()
    {
        // Arrange
        int pageNumber = 1;
        int pageSize = 10;
        var users = new List<UserDetailsDto>
        {
            new UserDetailsDto(new User { Id = Guid.NewGuid(), FirstName = "User1", UserName = "user1", Email = "user1@example.com" }),
            new UserDetailsDto(new User { Id = Guid.NewGuid(), FirstName = "User2", UserName = "user2", Email = "user2@example.com" })
        };
        var operationResult = new OperationResult<List<UserDetailsDto>>
        {
            IsSuccess = true,
            Value = users
        };
        _userServiceMock.Setup(service => service.GetUsersAsync(pageNumber, pageSize))
            .ReturnsAsync(operationResult);

        // Act
        var result = await _controller.GetUsersAsync(pageNumber, pageSize);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task DeactivateUserAsync_ShouldReturn204NoContent_WhenUserIsDeactivatedSuccessfully()
    {
        // Arrange
        string userId = "some-user-id";
        var operationResult = new OperationResult { IsSuccess = true };
        _userServiceMock.Setup(service => service.DeactivateUserAsync(userId))
            .ReturnsAsync(operationResult);
        _authorizationServiceMock.Setup(auth => auth.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(AuthorizationResult.Success());

        // Act
        var result = await _controller.DeactivateUserAsync(userId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ActivateUserAsync_ShouldReturn204NoContent_WhenUserIsActivatedSuccessfully()
    {
        // Arrange
        string userId = "some-user-id";
        var operationResult = new OperationResult { IsSuccess = true };
        _userServiceMock.Setup(service => service.ActivateUserAsync(userId))
            .ReturnsAsync(operationResult);
        _authorizationServiceMock.Setup(auth => auth.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
            .ReturnsAsync(AuthorizationResult.Success());

        // Act
        var result = await _controller.ActivateUserAsync(userId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturn204NoContent_WhenUserIsDeletedSuccessfully()
    {
        // Arrange
        string userId = "some-user-id";
        var operationResult = new OperationResult { IsSuccess = true };
        _userServiceMock.Setup(service => service.DeleteUserAsync(userId))
            .ReturnsAsync(operationResult);

        // Act
        var result = await _controller.DeleteUserAsync(userId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task AssignUserToRoleAsync_ShouldReturn204NoContent_WhenUserIsAssignedToRoleSuccessfully()
    {
        // Arrange
        var roleAssignment = (userId: "some-user-id", role: new List<string>() { "role" });
        var operationResult = new OperationResult { IsSuccess = true };
        _userServiceMock.Setup(service => service.AssignUserToRoleAsync(roleAssignment.userId, roleAssignment.role))
            .ReturnsAsync(operationResult);

        // Act
        var result = await _controller.AssignUserToRoleAsync(roleAssignment);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task RemoveUserFromRoleAsync_ShouldReturn204NoContent_WhenUserIsRemovedFromRoleSuccessfully()
    {
        // Arrange
        var roleAssignment = (userId: "some-user-id", role: "Admin");
        var operationResult = new OperationResult { IsSuccess = true };
        _userServiceMock.Setup(service => service.RemoveUserFromRoleAsync(roleAssignment.userId, roleAssignment.role))
            .ReturnsAsync(operationResult);

        // Act
        var result = await _controller.RemoveUserFromRoleAsync(roleAssignment);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}
