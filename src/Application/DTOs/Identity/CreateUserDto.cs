namespace Application.DTOs.Identity
{
	public class CreateUserDto
	{

		public required string UserName { get; set; }
		public required string Password { get; set; }
		public required string Email { get; set; }
		public required string FirstName { get; set; }
		public string? LastName { get; set; }
		public DateOnly? DateOfBirth { get; set; }

	}
}
