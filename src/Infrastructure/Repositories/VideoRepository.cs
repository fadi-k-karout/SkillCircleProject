using Application.Common.Interfaces.repos;
using Application.DTOs.Content;
using Domain.Models;
using Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VideoRepository : IVideoRepository
{
    private readonly ApplicationDbContext _context;

    public VideoRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<Video?> GetByIdAsync(Guid id)
    {
        return await _context.Videos.FindAsync(id);
    }

    public async Task AddAsync(Video entity)
    {
        await _context.Videos.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
    
    public async Task AddWithNoSaveAsync(Video video)
    {
        await _context.Videos.AddAsync(video);
    }

    public async Task UpdateAsync(Video entity)
    {
        _context.Videos.Update(entity);
        await _context.SaveChangesAsync();
    }
    public void  UpdateBatchWithNoSave(IEnumerable<Video> videos)
    {
        if (videos == null || !videos.Any())
            throw new ArgumentException("The list of videos to update cannot be null or empty.");

        _context.Videos.UpdateRange(videos);
    }

    public async Task DeleteAsync(Guid id)
    {
        var video = await _context.Videos.FindAsync(id);
        if (video != null)
        {
            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<(List<VideoDto> Videos, int TotalCount)> GetPaginatedByCourseIdAsync(Guid courseId, int? page = null, int? pageSize = null)
    {
        
        var totalCount = await _context.Videos
            .Where(v => v.CourseId == courseId)
            .CountAsync();

       
        var query = _context.Videos
            .Where(v => v.CourseId == courseId)
            .Include(v => v.Reviews)
            .ProjectToType<VideoDto>();

      
        if (page.HasValue && pageSize.HasValue)
        {
            query = query
                .Skip((page.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }

        var items = await query
            .AsNoTracking()
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<Video>> GetVideosByCourseIdAsync(Guid courseId)
    {
        return await _context.Videos
            .Where(video => video.CourseId == courseId)
            .ToListAsync();
    }
    public async Task<string?> GetOwnerIdAsync(Guid videoId)
    {
        
        var video = await _context.Videos
            .AsNoTracking() 
            .SingleOrDefaultAsync(c => c.Id == videoId);

        return video?.CreatorId.ToString(); 
    }
}
