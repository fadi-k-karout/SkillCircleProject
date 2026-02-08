namespace Domain.Models;

public class Course
{
    public  Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public string Description { get; set; }
    public required string Slug { get;  set; } // Slug property
    public Guid SkillId { get; set; }
    public Skill Skill { get; set; }
   
    public required Guid CreatorId { get; set; }
    public Status Status { get; set; } = new Status { CreatedAt = DateTime.UtcNow };
    public required bool IsPaid { get; set; }
    
    public decimal Price { get; set; }
    public required bool IsPrivate { get; set; }
    public ICollection<Video> Videos { get; set; } = new List<Video>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
   
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public User Creator { get; set; }
 
    
    public Course(){}
    
    /*public Course(string title, string description, string slug, Guid skillId, Guid creatorId)
    {
        Title = title;
        Description = description;
        Slug = slug;
        Videos = new List<Video>();
        Reviews = new List<Review>();
    }*/

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
    
    public void AddVideo(Video video)
    {
        if(Videos == null)
            throw new ArgumentNullException(nameof(Video),"Video cannot be null");
      
        if (!Videos.Contains(video)) Videos.Add(video);
        
    }

    public void RemoveVideo(Video video)
    {
        if (Videos == null)
            throw new ArgumentNullException(nameof(Video),"Videos cannot be null");
        
        if (Videos.Contains(video))  Videos.Remove(video);
       
    }
    
    public void AddReview(Review review)
    {
        if(Reviews == null)
            throw new ArgumentNullException(nameof(Review),"Review cannot be null");
      
        if (!Reviews.Contains(review)) Reviews.Add(review);
      
    }

    public void RemoveReview(Review review)
    {
        if (review == null)
            throw new ArgumentNullException(nameof(Review),"Review cannot be null");

       
        Reviews.Remove(review);

    }
    
    
}
