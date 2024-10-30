using Application.Common.Interfaces;
using Application.Common.Operation;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Controllers.Identity;
using Xunit;

namespace Web.Tests.ControllersTests
{
    public class RoleControllerTests
    {
        private readonly Mock<IRoleService> _roleServiceMock;
        private readonly RoleController _controller;

        public RoleControllerTests()
        {
            _roleServiceMock = new Mock<IRoleService>();
            _controller = new RoleController(_roleServiceMock.Object);
        }

        [Fact]
        public async Task CreateRoleAsync_ShouldReturnCreated_WhenRoleIsCreatedSuccessfully()
        {
            // Arrange
            string roleName = "Admin";
            string roleId = "guidkey";
            _roleServiceMock
                .Setup(service => service.CreateRoleAsync(roleName))
                .ReturnsAsync(OperationResult<string>.Success(roleId,SuccessTypes.Created));

            // Act
            var result = await _controller.CreateRoleAsync(roleName);

            // Assert
            result.Should().BeOfType<CreatedResult>();
            var createdResult = result as CreatedResult;
            createdResult.Should().NotBeNull();
            createdResult.Value.Should().BeEquivalentTo(roleId);
        }

        [Fact]
        public async Task CreateRoleAsync_ShouldReturnBadRequest_WhenRoleCreationFails()
        {
            // Arrange
            string roleName = "Admin";
            _roleServiceMock
                .Setup(service => service.CreateRoleAsync(roleName))
                .ReturnsAsync(OperationResult<string>.Failure("Role creation failed"));

            // Act
            var result = await _controller.CreateRoleAsync(roleName);

            // Assert
            result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task DeleteRoleAsync_ShouldReturnNoContent_WhenRoleIsDeletedSuccessfully()
        {
            // Arrange
            string roleId = "some-role-id";
            _roleServiceMock
                .Setup(service => service.DeleteRoleAsync(roleId))
                .ReturnsAsync(OperationResult.Success());

            // Act
            var result = await _controller.DeleteRoleAsync(roleId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteRoleAsync_ShouldReturnNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            string roleId = "non-existent-role-id";
            _roleServiceMock
                .Setup(service => service.DeleteRoleAsync(roleId))
                .ReturnsAsync(OperationResult.ResourceNotFound(nameof(roleId), roleId));

            // Act
            var result = await _controller.DeleteRoleAsync(roleId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetRolesAsync_ShouldReturnOk_WithListOfRoles()
        {
            // Arrange
            var roles = new List<RoleDto> { new RoleDto { Id = "role1", Name = "Admin" } };
            _roleServiceMock
                .Setup(service => service.GetRolesAsync())
                .ReturnsAsync(OperationResult<List<RoleDto>>.Success(roles));

            // Act
            var result = await _controller.GetRolesAsync();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(roles);
        }

        [Fact]
        public async Task GetRolesAsync_ShouldReturnServerError_WhenExceptionOccurs()
        {
            // Arrange
            _roleServiceMock
                .Setup(service => service.GetRolesAsync())
                .ReturnsAsync(OperationResult<List<RoleDto>>.Failure("Internal error"));

            // Act
            var result = await _controller.GetRolesAsync();

            // Assert
            result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }

        [Fact]
        public async Task GetRoleByIdAsync_ShouldReturnOk_WhenRoleExists()
        {
            // Arrange
            string roleId = "role-id";
            var roleDto = new RoleDto { Id = roleId, Name = "Admin" };
            _roleServiceMock
                .Setup(service => service.GetRoleByIdAsync(roleId))
                .ReturnsAsync(OperationResult<RoleDto>.Success(roleDto));

            // Act
            var result = await _controller.GetRoleByIdAsync(roleId);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
            okResult.Value.Should().BeEquivalentTo(roleDto);
        }

        [Fact]
        public async Task GetRoleByIdAsync_ShouldReturnNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            string roleId = "non-existent-role-id";
            _roleServiceMock
                .Setup(service => service.GetRoleByIdAsync(roleId))
                .ReturnsAsync(OperationResult<RoleDto>.ResourceNotFound(nameof(roleId), roleId));

            // Act
            var result = await _controller.GetRoleByIdAsync(roleId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task UpdateRoleAsync_ShouldReturnNoContent_WhenRoleIsUpdatedSuccessfully()
        {
            // Arrange
            string roleId = "role-id";
            string roleName = "UpdatedRole";
            _roleServiceMock
                .Setup(service => service.UpdateRoleAsync(roleId, roleName))
                .ReturnsAsync(OperationResult.Success());

            // Act
            var result = await _controller.UpdateRoleAsync(roleId, roleName);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateRoleAsync_ShouldReturnNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            string roleId = "non-existent-role-id";
            string roleName = "UpdatedRole";
            _roleServiceMock
                .Setup(service => service.UpdateRoleAsync(roleId, roleName))
                .ReturnsAsync(OperationResult.ResourceNotFound(nameof(roleId), roleId));

            // Act
            var result = await _controller.UpdateRoleAsync(roleId, roleName);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task UpdateRoleAsync_ShouldReturnBadRequest_WhenUpdateFails()
        {
            // Arrange
            string roleId = "role-id";
            string roleName = "UpdatedRole";
            _roleServiceMock
                .Setup(service => service.UpdateRoleAsync(roleId, roleName))
                .ReturnsAsync(OperationResult.Failure("Update failed"));

            // Act
            var result = await _controller.UpdateRoleAsync(roleId, roleName);

            // Assert
            result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(500);
        }
    }
}
