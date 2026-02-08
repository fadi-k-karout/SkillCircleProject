namespace Application.DTOs.Content;

public class SkillWithCoursesDto
{
    public SkillDto Skill { get; set; }
    public List<CourseDto> CourseDtos { get; set; }
    
    public int TotalCourseCount { get; set; }
}