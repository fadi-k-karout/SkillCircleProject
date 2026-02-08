namespace Domain.Models;

public class Status
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; } // Nullable for soft delete
    public bool IsSoftDeleted { get; set; }
}