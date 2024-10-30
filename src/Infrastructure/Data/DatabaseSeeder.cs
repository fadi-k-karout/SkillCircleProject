using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data;

public class DatabaseSeeder
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public DatabaseSeeder(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        // Ensure roles exist
        await SeedRolesAsync();

        // Ensure users exist (if you want to add default users)
        await SeedUsersAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[] { "student", "instructor", "admin", "moderator" };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        var users = new[]
        {
            new { Email = "admin@skillcricle.com", Password = "Admin@123", Role = "admin", FirstName = "Admin", LastName = "User" },
            new { Email = "student@skillcricle.com", Password = "Student@123", Role = "student", FirstName = "Student", LastName = "User" },
            new { Email = "instructor@skillcricle.com", Password = "Instructor@123", Role = "instructor", FirstName = "Instructor", LastName = "User" },
            new { Email = "moderator@skillcricle.com", Password = "Moderator@123", Role = "moderator", FirstName = "Moderator", LastName = "User" }
        };

        foreach (var userInfo in users)
        {
            // Check if the user already exists
            if (await _userManager.FindByEmailAsync(userInfo.Email) == null)
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = userInfo.Email,
                    Email = userInfo.Email,
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName
                };

                // Create the user with the specified password
                var result = await _userManager.CreateAsync(user, userInfo.Password);
                if (result.Succeeded)
                {
                    // Assign the user to the role
                    await _userManager.AddToRoleAsync(user, userInfo.Role);
                }
            }
        }
    }

}
