using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class User : IdentityUser<Guid>
{
	     public bool IsActive { get; set; } = true;
	     public string FirstName {  get; set; }
	     public string? LastName { get; set; }
	     public string? PhotoUrl { get; set; }
	     public DateOnly? DateOfBirth { get; set; }
	     
	     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	     
	     public ICollection<Skill> Skills { get; private set; } = new List<Skill>();
	     private readonly List<Video> _videos = new();
	     public IReadOnlyCollection<Video> Videos => _videos.AsReadOnly();
	     
	     // Navigational properties for reviews and ratings
	     private readonly List<Review> _reviews = new();
	     public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();


	     private readonly List<Course> _courses = new();
	     public IReadOnlyCollection<Course> Courses => _courses.AsReadOnly();
	     
	    
	     public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

