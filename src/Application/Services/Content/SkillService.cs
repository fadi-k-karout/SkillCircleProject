using Application.Common;
using Application.Common.Interfaces.Content;
using Application.Common.Interfaces.repos;
using Application.Common.Operation;
using Application.DTOs.Content;
using Domain.Models;
using Mapster;

namespace Application.Services.Content;

public class SkillService : ISkillService
{
    private readonly ISkillRepository _skillRepository;
   

    public SkillService(ISkillRepository skillRepository)
    {
        _skillRepository = skillRepository;
       
    }

    public async Task<OperationResult<SkillWithCoursesDto>> GetSkillWithCoursesById(Guid skillId)
    {
        var result = await _skillRepository.GetSkillWithCoursesByIdAsync(skillId);

        if (result == null)
        {
            return OperationResult<SkillWithCoursesDto>.ResourceNotFound("Skill", skillId);
        }

        return OperationResult<SkillWithCoursesDto>.Success(result);
    }

    public async Task<OperationResult> CreateSkill(SkillDto skillDto)
    {
        if (skillDto == null)
        {
            return OperationResult<Skill>.ArgumentNullError("Skill");
        }
        var slug = SlugHelper.GenerateSlug(skillDto.Name);

        var skill = new Skill(skillDto.Name, skillDto.Description, slug, new Status());

        await _skillRepository.AddAsync(skill);
        return OperationResult.Success();
    }

    public async Task<OperationResult> UpdateSkill(Guid skillId, SkillDto skillDto)
    {
        var skill = await _skillRepository.GetByIdAsync(skillId);
        if (skill == null)
        {
            return OperationResult<Skill>.ResourceNotFound("Skill", skillId);
        }

        skill.Name = skillDto.Name;
        skill.Description = skillDto.Description;
        skill.Status.UpdatedAt = DateTime.UtcNow;

        await _skillRepository.UpdateAsync(skill);
        return OperationResult.Success();
    }

    public async Task<OperationResult> SoftDeleteSkill(Guid skillId)
    {
        var skill = await _skillRepository.GetByIdAsync(skillId);
        if (skill == null)
        {
            return OperationResult<Skill>.ResourceNotFound("Skill", skillId);
        }

        skill.SoftDelete(); 
        await _skillRepository.UpdateAsync(skill);
        return OperationResult.Success();
    }

    public async Task<OperationResult<(List<SkillDto> Items, int TotalCount)>> GetPaginatedSkillsAsync(int? page = null, int? pageSize = null)
    {
        var (items, totalCount) = await _skillRepository.GetPaginatedSkillsAsync(page ?? 1, pageSize ?? 10);

        // Check if items is null or empty
        if (items == null || !items.Any())
        {
            return OperationResult<(List<SkillDto> Items, int TotalCount)>.ResourceNotFound("Skills", 0);
        }

        var skillDtos = items.Select(s => s.Adapt<SkillDto>()).ToList();
        return OperationResult<(List<SkillDto> Items, int TotalCount)>.Success((skillDtos, totalCount));
    }

    public async Task<OperationResult<List<SkillWithCoursesDto>>> GetAllSkillsWithCoursesAsync()
    {
        var skillsWithCourses = await _skillRepository.GetAllSkillsWithCoursesAsync();
        
        // Check if skillsWithCourses is null or empty
        if (skillsWithCourses == null || !skillsWithCourses.Any())
        {
            return OperationResult<List<SkillWithCoursesDto>>.ResourceNotFound("Skills", 0);
        }

        return OperationResult<List<SkillWithCoursesDto>>.Success(skillsWithCourses);
    }

    public async Task<OperationResult<List<SkillWithCoursesDto>>> GetAllSkillsWithPaginatedCoursesAsync(int page, int pageSize)
    {
        var skillsWithPaginatedCourses = await _skillRepository.GetAllSkillsWithPaginatedCoursesAsync(page, pageSize);
        
        // Check if skillsWithPaginatedCourses is null or empty
        if (skillsWithPaginatedCourses == null || !skillsWithPaginatedCourses.Any())
        {
            return OperationResult<List<SkillWithCoursesDto>>.ResourceNotFound("Skills", 0);
        }

        return OperationResult<List<SkillWithCoursesDto>>.Success(skillsWithPaginatedCourses);
    }

    public async Task<OperationResult> AddCreatorToSkillsAsync(Guid creatorId, List<Guid> skillIds)
    {
        if (skillIds == null || !skillIds.Any())
        {
            
        }

        var result = await _skillRepository.AddCreatorToSkillsAsync(creatorId, skillIds);
        if(!result)
            return OperationResult.ResourceNotFound("User", creatorId);
        
        return OperationResult.Success();

    }

    public async Task<OperationResult<List<SkillDto>>> GetCreatorSkillsById(Guid creatorId)
    {
        var skills = await _skillRepository.GetCreatorSkillsByIdAsync(creatorId);
        
        return OperationResult<List<SkillDto>>.Success(skills);
    }
}
