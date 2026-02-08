namespace Application.DTOs.Content;

public class VideoCreateUpdateDto
{
    public string Id { get; set; }
    public string Title { get; set; } // Title of the video
    public string Description { get; set; } // Description of the video
    public string ThumbnailTime { get; set; } = "0s"; // Thumbnail settings
    public bool IsPrivate { get; set; } // Is the video private?
    public bool IsPaid { get; set; } // Is the video paid?
    public string ProviderName { get; set; }
    public string providerVideoId { get; set; }
    public Guid CreatorId { get; set; }
    public Guid CourseId { get; set; } // Foreign Key to Course
}
