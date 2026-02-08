using Application.Common.Interfaces.repos;
using Application.DTOs.Content;
using Application.DTOs.Identity;
using Domain.Models;
using Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ApplicationDbContext _context;

    public CourseRepository(ApplicationDbContext context)
    {
        _context = context;
    }




    
 
    private async Task<CourseWithVideosDto> GetCourseByIdWithVideosAsync(Guid courseId, int? page = null, int? pageSize = null)
    {
        // Fetch the course with videos and reviews
        var course = await _context.Courses
            .Where(c => c.Id == courseId)
            .Include(c => c.Videos)
            .Include(c=>c.Creator)
            .Include(c => c.Reviews) 
            .SingleOrDefaultAsync();

        if (course == null)
            return null;

       
        var courseWithVideosDto = new CourseWithVideosDto
        {
            Course = new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Slug = course.Slug,
                Description = course.Description,
                IsPaid = course.IsPaid,
                Creator = new UserPublicInfoDto(course.Creator)
            },
            TotalVideoCount = course.Videos.Count, // Total videos in the course
            AverageRating = course.Reviews.Any() ? course.Reviews.Average(r => r.Rating) : null, // Calculate average rating if reviews exist
            Videos = course.Videos
                .OrderByDescending(v => v.Status.CreatedAt)
                .Skip((page.HasValue && pageSize.HasValue) ? (page.Value - 1) * pageSize.Value : 0)
                .Take(pageSize ?? course.Videos.Count) 
                .Select(v => new VideoDto
                {
                    Id = v.Id,
                    Title = v.Title,
                    Slug = v.Slug,
                    Description = v.Description,
                    ProviderName = v.ProviderName,
                    providerVideoId = v.ProviderVideoId,
                    DurationInSeconds = v.DurationInSeconds,
                    ThumbnailTime = v.ThumbnailTime,
                    IsPaid = v.IsPaid,
                    CourseId = v.CourseId
                })
                .ToList()
        };

        return courseWithVideosDto;
    }


public async Task<CourseWithVideosDto> GetCourseWithPaginatedVideosAsync(Guid courseId,int page, int pageSize)
{
    var courses = await GetCourseByIdWithVideosAsync(courseId ,page, pageSize);
    return courses;
}


public async Task<CourseWithVideosDto> GetCourseWithVideosAsync(Guid courseId)
{
    var courses = await GetCourseByIdWithVideosAsync(courseId);
    return courses;
}
    
    

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        return await _context.Courses.FindAsync(id);
    }

    public async Task AddAsync(Course entity)
    {
        await _context.Courses.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
    public async Task AddWithNoSaveAsync(Course entity)
    {
        await _context.Courses.AddAsync(entity);
    }

    public async Task UpdateAsync(Course entity)
    {
        _context.Courses.Update(entity);
        await _context.SaveChangesAsync();
    }
    public void UpdateWithNoSave(Course course)
    {
        _context.Courses.Update(course);
    }

    public async Task DeleteAsync(Guid id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course != null)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<(List<CourseDto> Courses, int TotalCount)> GetCoursesDetailsByCreatorIdAsync(Guid creatorId, int? page, int? pageSize)
    {
       
        var query = _context.Courses
            .Where(c => c.CreatorId == creatorId);

        var totalCount = 5; //await query.CountAsync();

      
        if (page.HasValue && pageSize.HasValue)
        {
            query = query
                .Skip((page.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }

        var courses = await query
            .Include(c =>c.Creator)
            .ProjectToType<CourseDto>()
            .ToListAsync();

        return (courses, totalCount);
    }

    public async Task<(List<CourseDto> Courses, int TotalCount)> GetCoursesBySkillIdAsync(Guid skillId, int? page, int? pageSize)
    {
        
        var query = _context.Courses
            .Where(c => c.SkillId == skillId);

        var totalCount = await query.CountAsync();

     
        if (page.HasValue && pageSize.HasValue)
        {
            query = query
                .Skip((page.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }

        var courses = await query
            .AsNoTracking()
            .ProjectToType<CourseDto>()
            .ToListAsync();

        return (courses, totalCount);
    }



    
    public async Task<string?> GetOwnerIdAsync(Guid courseId)
    {
        
        var course = await _context.Courses
            .AsNoTracking() 
            .SingleOrDefaultAsync(c => c.Id == courseId);

        return course?.CreatorId.ToString(); 
    }
}
