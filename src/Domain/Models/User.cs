using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class User : IdentityUser<Guid>
{
	     public bool IsActive { get; set; } = true;
	     public string? FirstName {  get; set; }
	     public string? LastName { get; set; }
	     public DateOnly? DateOfBirth { get; set; }
	     
	     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}

