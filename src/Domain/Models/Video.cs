namespace Domain.Models;

public class Video
{
    public Guid Id { get; set; } // Unique Identifier
    public required string Title { get; set; } // Title of the video
    public required string Slug { get; set; }
    public string Description { get; set; }
    public int DurationInSeconds { get; set; } // Duration of the video
    // Thumbnail settings
    public string ThumbnailTime { get; set; } = "0s";
    public Status Status { get; set; } = new();
    public bool IsPrivate { get;  set; }
    public bool IsPaid { get; set; }
    public required Guid CourseId { get; set; } // Foreign Key to Course
    public required Guid CreatorId { get; set; }
    public User Creator { get; set; }
    public Course Course { get; set; } // Navigation property for Course
    
    public required string ProviderVideoId { get; set; }
    public required string ProviderName { get; set; }
  
    
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    
    public Video(){}
    // Constructor to initialize a Video with essential properties
    // Constructor to initialize a Video with essential properties
    public Video(string title,string description, string slug, Guid courseId, Guid creatorId)
    {
        Title = title;
        Description = description;
        Slug = slug;
        CourseId = courseId;
        CreatorId = creatorId;
    }

    public bool SoftDelete()
    {
        if (Status.IsSoftDeleted) return false;
             
        Status.IsSoftDeleted = true;
        return true;
    }

    public bool MakePrivate()
    {
        if (IsPrivate) return false;
        
        IsPrivate = true;
        return true;
    }

    public bool MakePublic()
    {
        if (!IsPrivate) return false;
        
        IsPrivate = false;
        return true;
    }

    public bool MakePaid()
    {
        if (IsPaid) return false;
        
        IsPaid = true;
        return true;
    }

    public bool MakeFree()
    {
        if (!IsPaid) return false;
        
        IsPaid = false;
        return true;
    }
}