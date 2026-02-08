namespace Application.DTOs.Content;

public class SkillDto
{
    public Guid Id { get; set; } // Unique Identifier
    public string Name { get; set; } // Name of the skill
    public string Slug { get; set; } // Slug for the skill
    public string Description { get; set; } // Description of the skill
    
}
