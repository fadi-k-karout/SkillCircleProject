namespace Domain.Models;

public class Review
{
    public Guid Id { get; set; } // Unique Identifier
    public string Content { get; set; } // The content of the review
    public decimal Rating { get; set; }
    public Guid CourseId { get; set; } // Foreign Key to Course
    public Course Course { get; set; } // Navigation property for Course
    public Guid VideoId { get; set; }
    public Video Video { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Status Status { get; set; } = new();
    
    public Review(){}

    public Review(string content,decimal rating, Guid courseId, Guid userId, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Content = content;
        Rating = rating;
        CourseId = courseId;
        UserId = userId;
    }
    
    public bool SoftDelete()
    {
        if (Status.IsSoftDeleted) return false;
             
        Status.IsSoftDeleted = true;
        return true;
    }

}