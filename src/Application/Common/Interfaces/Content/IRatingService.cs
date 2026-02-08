using Application.Common.Operation;
using Application.DTOs.Content;

namespace Application.Common.Interfaces.Content;

public interface IRatingService
{
    Task<OperationResult<RatingDto>> GetRatingForCourseAsync(string courseId);
    Task<OperationResult> CreateOrUpdateRatingAsync(RatingDto ratingDto);
}
