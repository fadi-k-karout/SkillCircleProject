using System.Security.Claims;
using Application.Common.Interfaces;
using Application.Common.Operation;
using Application.Services.Identity;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.ServicesTests;

public class AuthenticationServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<ITokenGenerator> _tokenGeneratorMock;
    private readonly AuthenticationService _authenticationService;

    public AuthenticationServiceTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object,
            null, null, null, null, null, null, null, null
        );

        _tokenGeneratorMock = new Mock<ITokenGenerator>();
        _authenticationService = new AuthenticationService(_userManagerMock.Object, _tokenGeneratorMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        var email = "test@example.com";
        var password = "Password123!";

        _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync((User)null);

        var result = await _authenticationService.LoginAsync(email, password);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid login attempt.", result.ErrorMessage);
        Assert.Equal(ErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenPasswordInvalid()
    {
        var email = "test@example.com";
        var password = "WrongPassword";
        var user = new User { Email = email };

        _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, password)).ReturnsAsync(false);

        var result = await _authenticationService.LoginAsync(email, password);

        Assert.False(result.IsSuccess);
        Assert.Equal("Invalid login attempt.", result.ErrorMessage);
        Assert.Equal(ErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenUserIsLockedOut()
    {
        var email = "test@example.com";
        var password = "Password123!";
        var user = new User { Email = email };

        _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, password)).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(true);

        var result = await _authenticationService.LoginAsync(email, password);

        Assert.False(result.IsSuccess);
        Assert.Equal("User is locked out.", result.ErrorMessage);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenLoginIsValid()
    {
        var email = "test@example.com";
        var password = "Password123!";
        var user = new User { Email = email, UserName = "testuser" };
        var roles = new List<string> { "User" };
        var token = "generated_token";

        _userManagerMock.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, password)).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.IsLockedOutAsync(user)).ReturnsAsync(false);
        _userManagerMock.Setup(x => x.IsEmailConfirmedAsync(user)).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);
        // Use explicit parameters instead of a tuple
        _tokenGeneratorMock.Setup(x => x.GenerateToken(It.IsAny<string>(),  It.IsAny<List<string>>()))
            .Returns(token);

        var result = await _authenticationService.LoginAsync(email, password);

        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id.ToString(), result.Value.UserId);
        Assert.Equal(token, result.Value.Token);
    }
[Fact]
 public async Task ExternalLoginAsync_ShouldReturnFailure_WhenUserCreationFails()
{
    var info = new ExternalLoginInfo(
        new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, "test@example.com")
        })),
        "Google", // LoginProvider
        "google_provider_key", // ProviderKey
        null // AuthenticationMethod (can be null)
    );

    _userManagerMock.Setup(x => x.FindByLoginAsync(info.LoginProvider, info.ProviderKey)).ReturnsAsync((User)null);
    _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed to create user." }));

    var result = await _authenticationService.ExternalLoginAsync(info);

    Assert.False(result.IsSuccess);
    Assert.Equal("External login failure, missing required claims", result.ErrorMessage);
}
[Fact]
public async Task ExternalLoginAsync_ShouldReturnSuccess_WhenUserIsCreated()
{
    var info = new ExternalLoginInfo(
        new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.GivenName, "Test"),
            new Claim(ClaimTypes.Surname, "User")
        })),
        "Google", // LoginProvider
        "google_provider_key", // ProviderKey
        null // AuthenticationMethod
    );

    // Use a fixed user ID for consistency in tests.
    var fixedUserId = Guid.NewGuid(); // The ID you want to assert later.
    var user = new User
    {
        // Don't assign Id here; we will set it in the CreateAsync mock.
        UserName = "test@example.com",
        Email = "test@example.com",
        FirstName = "Test",
        LastName = "User"
    };

    var roles = new List<string> { "User" };
    var token = "generated_token";

    // Setup mock behavior for user manager.
    _userManagerMock.Setup(x => x.FindByLoginAsync(info.LoginProvider, info.ProviderKey)).ReturnsAsync((User)null);
    _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
        .ReturnsAsync(IdentityResult.Success)
        .Callback<User>(u => 
        {
            // Assign fixed ID to the created user.
            u.Id = fixedUserId; // Ensure the created user has the fixed ID.
        });

    _userManagerMock.Setup(x => x.AddLoginAsync(It.IsAny<User>(), info)).ReturnsAsync(IdentityResult.Success);
    
    // Setup to return the expected roles
    _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(roles);
    
    _tokenGeneratorMock.Setup(x => x.GenerateToken(It.IsAny<string>(),  It.IsAny<List<string>>()))
        .Returns(token);

    var result = await _authenticationService.ExternalLoginAsync(info);

    Assert.True(result.IsSuccess);
    Assert.Equal(fixedUserId.ToString(), result.Value.UserId); // Assert against the fixed user ID.
    Assert.Equal(token, result.Value.Token);
}

}
