namespace Domain.Models;

public class Payment
{
    public required Guid Id { get; set; } // Primary key, if using EF Core, for example
    public required Guid CourseId { get; set; } // Foreign key to the course
    public required Guid UserId { get; set; } // Foreign key to the user
    public required decimal Amount { get; set; } // Amount paid
    public  bool IsPaid { get; set; } // Indicates whether the payment has been made
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp of when the payment was created
    public DateTime? PaidAt { get; set; } // Timestamp of when the payment was completed (nullable)
    public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    
    // Navigation properties
    public virtual Course Course { get; set; } // Navigation property to the Course
    public virtual User User { get; set; } // Navigation property to the User

    public bool Pay()
    {
        if (IsPaid) return false;
        
        IsPaid = true;
        return IsPaid;
    }
}
