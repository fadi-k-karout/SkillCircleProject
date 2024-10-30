namespace Application.DTOs.Identity
{
	public class UpdateUserProfileDto
	{
		public required string UserId { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? UserName { get; set; }
		public string? Email { get; set; }

	}
}
