using Application.Common.Interfaces.repos;
using Application.DTOs.Content;
using Application.DTOs.Identity;
using Domain.Models;
using Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SkillRepository : ISkillRepository
{
    private readonly ApplicationDbContext _context;

    public SkillRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Skill>> GetAllAsIEnumerableAsync()
    {
        return await _context.Skills.ToListAsync();
    }

    public async Task<IQueryable<Skill>> GetAllAsQueryableAsync()
    {
        return await Task.FromResult(_context.Skills.AsQueryable());

    }

    public async Task<Skill?> GetByIdAsync(Guid id)
    {

        return await _context.Skills.FindAsync(id);
    }

    public async Task<SkillWithCoursesDto?> GetSkillWithCoursesByIdAsync(Guid skillId)
    {
        var skillWithCourses = await _context.Skills
            .Where(s => s.Id == skillId)
            .Include(s => s.Courses)
            .ThenInclude(c => c.Creator)
            .FirstOrDefaultAsync();

        if (skillWithCourses == null)
        {
            // Skill not found, return null
            return null;
        }


        var result = new SkillWithCoursesDto
        {
            Skill = new SkillDto
            {
                Id = skillWithCourses.Id,
                Name = skillWithCourses.Name,
                Slug = skillWithCourses.Slug,
                Description = skillWithCourses.Description,
            },
            TotalCourseCount = skillWithCourses.Courses.Count,
            CourseDtos = skillWithCourses.Courses.Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Slug = c.Slug,
                Description = c.Description,
                IsPaid = c.IsPaid,
                Creator = c.Creator == null ? null : new UserPublicInfoDto
                {
                    
                    FirstName = c.Creator.FirstName,
                    LastName = c.Creator.LastName,
                    PhotoUrl = c.Creator.PhotoUrl,
                    CreatedAt = c.Creator.CreatedAt,
                   
                }
            }).ToList()
        };

        return result;
    }



    // Helper method for retrieving Skills with optional pagination on Courses
private async Task<List<SkillWithCoursesDto>> GetSkillsWithCoursesAsync(int? page = null, int? pageSize = null)
{
    // Retrieve skills with their courses
    var skillsWithCourses = await _context.Skills
        .Include(s => s.Courses)
        .ToListAsync();

    
    var skills = skillsWithCourses
        .Select(s => new SkillWithCoursesDto
        {
            Skill = new SkillDto
            {
                Id = s.Id,
                Name = s.Name,
                Slug = s.Slug,
                Description = s.Description,
            },
            TotalCourseCount = s.Courses.Count, 
            CourseDtos = s.Courses
                .OrderBy(c => c.Id)
                .Skip((page.HasValue && pageSize.HasValue) ? (page.Value - 1) * pageSize.Value : 0)
                .Take(pageSize ?? s.Courses.Count) 
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Slug = c.Slug,
                    Description = c.Description,
                    IsPaid = c.IsPaid,
                })
                .ToList()
        })
        .ToList();

    return skills;
}

// Method for getting all skills with paginated courses and course count for each skill
public async Task<List<SkillWithCoursesDto>> GetAllSkillsWithPaginatedCoursesAsync(int page, int pageSize)
{
   
    var skills = await GetSkillsWithCoursesAsync(page, pageSize);
    return skills;
}

// Method for getting all skills with all courses and course count (no pagination)
public async Task<List<SkillWithCoursesDto>> GetAllSkillsWithCoursesAsync()
{
    var skills = await GetSkillsWithCoursesAsync();
    return skills;
}



    public async Task AddAsync(Skill entity)
    {
        await _context.Skills.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Skill entity)
    {
        _context.Skills.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill != null)
        {
            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<(List<SkillDto> Skills, int TotalCount)> GetPaginatedSkillsAsync(int? page, int? pageSize)
    {
        var totalCount = await _context.Skills.CountAsync();
    
       
        if (page == null || pageSize == null)
        {
            var allSkills = await _context.Skills
                .AsNoTracking()
                .ProjectToType<SkillDto>()
                .ToListAsync();
        
            return (allSkills, totalCount);
        }

       
        var skills = await _context.Skills
            .ProjectToType<SkillDto>()
            .AsNoTracking()
            .Skip((page.Value - 1) * pageSize.Value)
            .Take(pageSize.Value)
            .ToListAsync();

        return (skills, totalCount);
    }

    
    public async Task<List<SkillDto>> GetCreatorSkillsByIdAsync(Guid creatorId)
    {
        return await _context.Users
            .Where(u => u.Id == creatorId)
            .SelectMany(u => u.Skills)
            .ProjectToType<SkillDto>()
            .ToListAsync();
    }
    public async Task<bool> AddCreatorToSkillsAsync(Guid creatorId, List<Guid> skillIds)
    {
        var user = await _context.Users.Include(u => u.Skills).FirstOrDefaultAsync(u => u.Id == creatorId);

        if (user == null)
            return false; 

        var skills = await _context.Skills.Where(s => skillIds.Contains(s.Id)).ToListAsync();

        foreach (var skill in skills)
        {
            user.Skills.Add(skill);
        }

        await _context.SaveChangesAsync();
        return true;
    }

}
