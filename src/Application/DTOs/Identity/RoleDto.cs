using Microsoft.AspNetCore.Identity;

namespace Application.DTOs.Identity
{
	public class RoleDto
	{
		public string Id {  get; set; }
		public string? Name { get; set; }
		public RoleDto(IdentityRole<Guid> identityRole)
		{
			Id = identityRole.Id.ToString();
			Name = identityRole.Name;
		
		}
		public RoleDto(){}
		
	}
}
