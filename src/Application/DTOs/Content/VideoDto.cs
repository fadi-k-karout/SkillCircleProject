namespace Application.DTOs.Content;

public class VideoDto
{
    public Guid Id { get; set; } // Unique Identifier
    public string Title { get; set; } // Title of the video
    public string Slug { get; set; } // Slug for the video
    public string Description { get; set; } // Description of the video
    public string ProviderName { get; set; }
    public string providerVideoId { get; set; }
    public int DurationInSeconds { get; set; } // Duration of the video
    public string ThumbnailTime { get; set; } = "0s"; // Thumbnail settings
    public bool IsPaid { get; set; } // Is the video paid?
    public decimal AverageRating { get; set; }
    public Guid CourseId { get; set; } // Foreign Key to Course
}
