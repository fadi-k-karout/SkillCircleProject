using Application.Common.Interfaces.repos;
using Application.DTOs.Content;
using Domain.Models;
using Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }



    public async Task<Review?> GetByIdAsync(Guid id)
    {
        return await _context.Reviews.FindAsync(id);
    }

    public async Task AddAsync(Review entity)
    {
        await _context.Reviews.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Review entity)
    {
        _context.Reviews.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<(List<ReviewDto> Reviews, int TotalCount)> GetReviewsByCourseIdAsync(Guid courseId, int? page = null, int? pageSize = null)
    {
        
        var totalCount = await _context.Reviews
            .Where(r => r.CourseId == courseId)
            .CountAsync();

       
        var query = _context.Reviews
            .Where(r => r.CourseId == courseId)
            .Include(r => r.User)
            .ProjectToType<ReviewDto>()
            .AsNoTracking();

        
        if (page.HasValue && pageSize.HasValue)
        {
            query = query
                .Skip((page.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }

        var items = await query
            .ToListAsync();

        return (items, totalCount);
    }
    
    public async Task<(List<ReviewDto> Reviews, int TotalCount)> GetReviewsByVideoIdAsync(Guid videoId, int? page = null, int? pageSize = null)
    {
        
        var totalCount = await _context.Reviews
            .Where(r => r.VideoId == videoId)
            .CountAsync();

       
        var query = _context.Reviews
            .Where(r => r.VideoId == videoId)
            .Include(r => r.User)
            .ProjectToType<ReviewDto>()
            .AsNoTracking();

        
        if (page.HasValue && pageSize.HasValue)
        {
            query = query
                .Skip((page.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }

        var items = await query
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<string?> GetOwnerIdAsync(Guid reviewId)
    {
        
        var review = await _context.Reviews
            .AsNoTracking() 
            .SingleOrDefaultAsync(c => c.Id == reviewId);

        return review?.UserId.ToString(); 
    }

}
