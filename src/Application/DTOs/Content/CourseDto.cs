using Application.DTOs.Identity;

namespace Application.DTOs.Content;

public class CourseDto
{
    public Guid Id { get; set; } 
    public string Title { get; set; } 
    public string Description { get; set; } 
    public string Slug { get; set; } 
    public int NumberOfReviews { get; set; }
    public decimal AverageRating { get; set; }
    public UserPublicInfoDto Creator { get; set; }
    public bool IsPaid { get; set; } 
    
    public decimal? Price  { get; set; }
}
