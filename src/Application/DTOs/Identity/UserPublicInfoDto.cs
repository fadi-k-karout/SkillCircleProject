using Domain.Models;

namespace Application.DTOs.Identity;

public class UserPublicInfoDto
{
    public string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime? CreatedAt { get; set; }
    
   public UserPublicInfoDto(){}
    public UserPublicInfoDto(User user)
    {
        FirstName = user.FirstName;
        LastName = user.LastName;
        PhotoUrl = user.PhotoUrl;
        
    }
}