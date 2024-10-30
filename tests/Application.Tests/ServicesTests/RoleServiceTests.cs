using Application.Common.Operation;
using Application.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Moq;


namespace Application.Tests;
public class RoleServiceTests
{
    private readonly Mock<RoleManager<IdentityRole<Guid>>> _roleManagerMock;
    private readonly RoleService _roleService;

    public RoleServiceTests()
    {
        // Mock the RoleStore and the RoleManager
        var store = new Mock<IRoleStore<IdentityRole<Guid>>>();
        _roleManagerMock = new Mock<RoleManager<IdentityRole<Guid>>>(store.Object, null, null, null, null);
        
        _roleService = new RoleService(_roleManagerMock.Object);
    }

    #region CreateRoleAsync Tests

    [Fact]
    public async Task CreateRoleAsync_ShouldReturnSuccess_WhenRoleIsCreated()
    {
        // Arrange
        _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole<Guid>>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _roleService.CreateRoleAsync("Admin");

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CreateRoleAsync_ShouldReturnFailure_WhenRoleCreationFails()
    {
        // Arrange
        var error = new IdentityError { Description = "Failed to create role." };
        _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole<Guid>>()))
            .ReturnsAsync(IdentityResult.Failed(error));

        // Act
        var result = await _roleService.CreateRoleAsync("Admin");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
    }

    #endregion

    #region DeleteRoleAsync Tests

    [Fact]
    public async Task DeleteRoleAsync_ShouldReturnSuccess_WhenRoleIsDeleted()
    {
        // Arrange
        var role = new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "User" };
        _roleManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(role);
        _roleManagerMock.Setup(x => x.DeleteAsync(role)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _roleService.DeleteRoleAsync(role.Id.ToString());

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteRoleAsync_ShouldReturnNotFound_WhenRoleDoesNotExist()
    {
        // Arrange
        _roleManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityRole<Guid>)null);

        // Act
        var result = await _roleService.DeleteRoleAsync(Guid.NewGuid().ToString());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task DeleteRoleAsync_ShouldReturnFailure_WhenRoleDeletionFails()
    {
        // Arrange
        var role = new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "User" };
        var error = new IdentityError { Description = "Failed to delete role." };
        _roleManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(role);
        _roleManagerMock.Setup(x => x.DeleteAsync(role)).ReturnsAsync(IdentityResult.Failed(error));

        // Act
        var result = await _roleService.DeleteRoleAsync(role.Id.ToString());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
    }

    #endregion

    #region GetRolesAsync Tests


 
    [Fact]
    public async Task GetRolesAsync_ShouldReturnListOfRoles_WhenRolesExist()
    {
        // Arrange
        var roles = new List<IdentityRole<Guid>>
        {
            new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Admin" },
            new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "User" }
        }.AsQueryable();

        // Create a mock DbSet from the roles
        var mockRolesDbSet = new Mock<DbSet<IdentityRole<Guid>>>();
        mockRolesDbSet.As<IQueryable<IdentityRole<Guid>>>().Setup(m => m.Provider).Returns(roles.Provider);
        mockRolesDbSet.As<IQueryable<IdentityRole<Guid>>>().Setup(m => m.Expression).Returns(roles.Expression);
        mockRolesDbSet.As<IQueryable<IdentityRole<Guid>>>().Setup(m => m.ElementType).Returns(roles.ElementType);
        mockRolesDbSet.As<IQueryable<IdentityRole<Guid>>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

        // Setup the mock RoleManager to return the mock DbSet
        _roleManagerMock.Setup(rm => rm.Roles).Returns(mockRolesDbSet.Object);

        // Act
        var result = await _roleService.GetRolesAsync();

        // Assert
        Assert.True(result.IsSuccess, result.ErrorMessage ?? "Expected operation to succeed.");
        Assert.Equal(2, result.Value.Count);
        Assert.Contains(result.Value, r => r.Name == "Admin");
        Assert.Contains(result.Value, r => r.Name == "User");
    }


    [Fact]
    public async Task GetRolesAsync_ShouldReturnFailure_WhenExceptionOccurs()
    {
        // Arrange
        _roleManagerMock.Setup(x => x.Roles).Throws(new Exception("Database error"));

        // Act
        var result = await _roleService.GetRolesAsync();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
    }

    #endregion

    #region GetRoleByIdAsync Tests

    [Fact]
    public async Task GetRoleByIdAsync_ShouldReturnRole_WhenRoleExists()
    {
        // Arrange
        var role = new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "User" };
        _roleManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(role);

        // Act
        var result = await _roleService.GetRoleByIdAsync(role.Id.ToString());

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("User", result.Value.Name);
    }

    [Fact]
    public async Task GetRoleByIdAsync_ShouldReturnNotFound_WhenRoleDoesNotExist()
    {
        // Arrange
        _roleManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityRole<Guid>)null);

        // Act
        var result = await _roleService.GetRoleByIdAsync(Guid.NewGuid().ToString());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    #endregion

    #region UpdateRoleAsync Tests

    [Fact]
    public async Task UpdateRoleAsync_ShouldReturnSuccess_WhenRoleIsUpdated()
    {
        // Arrange
        var role = new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "User" };
        _roleManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(role);
        _roleManagerMock.Setup(x => x.UpdateAsync(role)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _roleService.UpdateRoleAsync(role.Id.ToString(), "UpdatedUser");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("UpdatedUser", role.Name);
    }

    [Fact]
    public async Task UpdateRoleAsync_ShouldReturnFailure_WhenRoleUpdateFails()
    {
        // Arrange
        var role = new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "User" };
        var error = new IdentityError { Description = "Failed to update role." };
        _roleManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(role);
        _roleManagerMock.Setup(x => x.UpdateAsync(role)).ReturnsAsync(IdentityResult.Failed(error));

        // Act
        var result = await _roleService.UpdateRoleAsync(role.Id.ToString(), "UpdatedUser");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
    }

    #endregion
}
