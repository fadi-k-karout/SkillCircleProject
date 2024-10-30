namespace Application.DTOs.Identity
{
	public class UpdatePasswordDto
	{
		public string UserId { get; set; }
		public string CurrentPassword { get; set; }
		public string NewPassword { get; set; }
		
	}
}
