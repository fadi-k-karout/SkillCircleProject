using Application.Common.Operation;
using Application.DTOs.Content;

namespace Application.Common.Interfaces.Content;

public interface ISkillService
{
    Task<OperationResult<SkillWithCoursesDto>> GetSkillWithCoursesById(Guid skillId);
    Task<OperationResult> CreateSkill(SkillDto skillDto);
    Task<OperationResult> UpdateSkill(Guid skillId, SkillDto skillDto);
    Task<OperationResult> SoftDeleteSkill(Guid skillId);

    Task<OperationResult<(List<SkillDto> Items, int TotalCount)>> GetPaginatedSkillsAsync(int? page = null,
        int? pageSize = null);
    Task<OperationResult<List<SkillWithCoursesDto>>> GetAllSkillsWithCoursesAsync();
    Task<OperationResult<List<SkillWithCoursesDto>>> GetAllSkillsWithPaginatedCoursesAsync(int page, int pageSize);
    Task<OperationResult> AddCreatorToSkillsAsync(Guid creatorId, List<Guid> skillIds);
}
