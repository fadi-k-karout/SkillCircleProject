using Application.DTOs.Identity;

namespace Application.DTOs.Content;

public class ReviewDto
{
    public Guid Id { get; set; } // Unique Identifier
    public string Content { get; set; } // The content of the review
    public decimal Rating { get; set; }
    public Guid CourseId { get; set; } // Foreign Key to Course
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public UserPublicInfoDto User { get; set; }
}
