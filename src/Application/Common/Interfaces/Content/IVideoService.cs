using Application.Common.Operation;
using Application.DTOs.Content;
using Domain.Models;

namespace Application.Common.Interfaces.Content;

public interface IVideoService
{
    Task<OperationResult<VideoDto>> GetVideoByIdAsync(Guid id);
    Task<OperationResult> CreateVideoAsync(VideoCreateUpdateDto videoDto);
    Task<OperationResult> UpdateVideoAsync(Guid id, VideoCreateUpdateDto videoDto);
    Task<OperationResult> DeleteVideoAsync(Guid id);
    Task<OperationResult> SoftDeleteVideoAsync(Guid videoId);
    Task<OperationResult> MakeVideoPrivateAsync(Guid videoId);
    Task<OperationResult> MakeVideoPublicAsync(Guid videoId);
    Task<OperationResult> MakeVideoPaidAsync(Guid videoId);
    Task<OperationResult> MakeVideoFreeAsync(Guid videoId);
    Task<OperationResult<(List<VideoDto> Items, int TotalCount)>> GetPaginatedByCourseIdAsync
        (Guid courseId, int? page = null, int? pageSize = null);


}
