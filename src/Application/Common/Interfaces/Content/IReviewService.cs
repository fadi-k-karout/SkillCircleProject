using Application.Common.Operation;
using Application.DTOs.Content;

namespace Application.Common.Interfaces.Content;

public interface IReviewService
{
    Task<OperationResult<(List<ReviewDto> Reviews, int TotalCount)>> GetReviewsByCourseIdAsync(Guid courseId,
        int? page = null, int? pageSize = null);

    Task<OperationResult<(List<ReviewDto> Reviews, int TotalCount)>> GetReviewsByVideoIdAsync
        (Guid videoId, int? page = null, int? pageSize = null);
    
    Task<OperationResult> CreateReviewAsync(ReviewCreateUpdateDto reviewDto);
    Task<OperationResult> UpdateReviewAsync(Guid reviewId, ReviewCreateUpdateDto reviewDto);
    Task<OperationResult> SoftDeleteReviewAsync(Guid reviewId);

}
