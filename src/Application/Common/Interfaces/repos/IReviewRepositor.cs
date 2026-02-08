using Application.DTOs.Content;
using Domain.Models;

namespace Application.Common.Interfaces.repos;

public interface IReviewRepository : IRepository<Review>
{
    Task<(List<ReviewDto> Reviews, int TotalCount)> GetReviewsByCourseIdAsync(Guid courseId, int? page = null,
        int? pageSize = null);
    Task<(List<ReviewDto> Reviews, int TotalCount)> GetReviewsByVideoIdAsync
        (Guid videoId, int? page = null, int? pageSize = null);
}