using Application.Common.Operation;
using Application.DTOs.Content;

namespace Application.Common.Interfaces.Content;

public interface ICourseService
{
    Task<OperationResult<CourseWithVideosDto>> GetCourseWithVideosAsync(Guid courseId);
    Task<OperationResult> CreateCourseAsync(CourseCreateUpdateDto courseDto);
    Task<OperationResult> UpdateCourseAsync(Guid courseId, CourseCreateUpdateDto courseDto);
    Task<OperationResult> DeleteCourseAsync(Guid courseId);
    Task<OperationResult> MakeCoursePaidAsync(Guid courseId);
    Task<OperationResult> MakeCourseFreeAsync(Guid courseId);
    Task<OperationResult> MakeCoursePrivateAsync(Guid courseId);
    Task<OperationResult> MakeCoursePublicAsync(Guid courseId);

    Task<OperationResult<(List<CourseDto> Courses, int TotalCount)>> GetCoursesBySkillIdAsync
        (Guid skillId, int? page = null, int? pageSize = null);
}
