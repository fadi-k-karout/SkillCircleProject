namespace Application.DTOs.Content;

public class CourseWithVideosDto
{
    public CourseDto Course { get; set; }
    public List<VideoDto> Videos { get; set; }
    public int TotalVideoCount { get; set; }
    public decimal? AverageRating { get; set; }
}