namespace Application.DTOs.Content;

public class ReviewCreateUpdateDto
{
    public string Content { get; set; } // The content of the review
    public decimal Rating { get; set; }
    
    public Guid? VideoId { get; set; }
    public Guid CourseId { get; set; } // Foreign Key to Course
    public Guid UserId { get; set; } // Foreign Key to User
}
