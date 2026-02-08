using Application.DTOs.Content;
using Domain.Models;

namespace Application.Common.Interfaces.repos;

public interface ICourseRepository : IRepository<Course>
{
     Task AddWithNoSaveAsync(Course course);
     void UpdateWithNoSave(Course course);

    Task<CourseWithVideosDto> GetCourseWithVideosAsync(Guid courseId);
    Task<CourseWithVideosDto> GetCourseWithPaginatedVideosAsync(Guid courseId ,int page, int pageSize);

    Task<(List<CourseDto> Courses, int TotalCount)> GetCoursesDetailsByCreatorIdAsync
        (Guid creatorId, int? page,int? pageSize);

    Task<(List<CourseDto> Courses, int TotalCount)> GetCoursesBySkillIdAsync
        (Guid creatorId, int? page, int? pageSize);
}