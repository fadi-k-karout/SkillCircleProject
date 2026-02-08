namespace Application.DTOs.Content;

public class RatingDto
{
    public Guid Id { get; set; } // Unique Identifier
    public int Value { get; set; } // Rating value (e.g., 1-5 stars)
    public Guid CourseId { get; set; } // Foreign Key to Course
    public Guid UserId { get; set; } // Foreign Key to User
}
