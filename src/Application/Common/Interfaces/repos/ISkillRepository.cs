using Application.DTOs.Content;
using Domain.Models;

namespace Application.Common.Interfaces.repos;

public interface ISkillRepository : IRepository<Skill>
{
    Task<Skill?> IRepository<Skill>.GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
    Task<string?> IRepository<Skill>.GetOwnerIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    Task<Skill?> GetByIdAsync(Guid id);
    Task<SkillWithCoursesDto?> GetSkillWithCoursesByIdAsync(Guid skillId);
    Task<List<SkillWithCoursesDto>> GetAllSkillsWithCoursesAsync();
    Task<List<SkillWithCoursesDto>> GetAllSkillsWithPaginatedCoursesAsync(int page, int pageSize);

    Task<List<SkillDto>> GetCreatorSkillsByIdAsync(Guid creatorId);
    Task<bool> AddCreatorToSkillsAsync(Guid creatorId, List<Guid> skillIds);
    Task<(List<SkillDto> Skills, int TotalCount)> GetPaginatedSkillsAsync(int? page, int? pageSize);
}