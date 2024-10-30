using Domain.Models;

namespace Application.DTOs.Identity
{
	public class UserDetailsDto
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string? LastName { get; set; }
		public  string UserName { get; set; }
		public  string Email { get; set; }


		// Constructor to create UserDetailsDto from ApplicationUser
		public UserDetailsDto(User user)
		{
			Id = user.Id.ToString();
			FirstName = user.FirstName!;
			LastName = user.LastName;
			UserName = user.UserName;
			Email = user.Email!;
			
		}
	}
}
