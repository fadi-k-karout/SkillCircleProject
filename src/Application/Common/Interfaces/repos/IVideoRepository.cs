using Application.DTOs.Content;
using Domain.Models;

namespace Application.Common.Interfaces.repos;

public interface IVideoRepository : IRepository<Video>
{
    Task AddWithNoSaveAsync(Video video);
    Task<(List<VideoDto> Videos, int TotalCount)> GetPaginatedByCourseIdAsync(Guid courseId, int? page, int? pageSize);
    Task<List<Video>> GetVideosByCourseIdAsync(Guid courseId);
    void UpdateBatchWithNoSave(IEnumerable<Video> videos);

}