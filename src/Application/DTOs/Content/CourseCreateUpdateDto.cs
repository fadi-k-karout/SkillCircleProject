namespace Application.DTOs.Content;

public class CourseCreateUpdateDto
{
    public string Title { get; set; } // Title of the course
    public string Description { get; set; } // Description of the course
    public Guid SkillId { get; set; } // Foreign Key to Skill
    public Guid CreatorId { get; set; } // Foreign Key to User
    public bool IsPaid { get; set; } // Is the course paid?
    
    public decimal Price { get; set; }
    public bool IsPrivate { get; set; }
    public List<VideoCreateUpdateDto> Videos;
}
