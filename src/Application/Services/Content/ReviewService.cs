using Application.Common.Interfaces.Content;
using Application.Common.Interfaces.repos;
using Application.Common.Operation;
using Application.DTOs.Content;
using Application.DTOs.Identity;
using Domain.Models;

namespace Application.Services.Content;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IReviewRepository reviewRepository, IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult> CreateReviewAsync(ReviewCreateUpdateDto reviewDto)
    {
        if (reviewDto == null)
            return OperationResult.ArgumentNullError(nameof(reviewDto));
        
        var status = new Status
        {
            CreatedAt = DateTime.UtcNow,
        };

        var review = new Review(reviewDto.Content, reviewDto.Rating,
            reviewDto.CourseId, reviewDto.UserId);
        

        await _reviewRepository.AddAsync(review);
       

        return OperationResult.Success();
    }

    public async Task<OperationResult> UpdateReviewAsync(Guid reviewId, ReviewCreateUpdateDto reviewDto)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null)
            return OperationResult.ResourceNotFound(nameof(review), reviewId);

        review.Rating = reviewDto.Rating;
        review.Content = reviewDto.Content;
        review.Status.UpdatedAt = DateTime.UtcNow;

        await _reviewRepository.UpdateAsync(review);
        

        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteReviewAsync(Guid reviewId)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null)
            return OperationResult.ResourceNotFound(nameof(review), reviewId);

        await _reviewRepository.DeleteAsync(reviewId);
      

        return OperationResult.Success();
    }
    public async Task<OperationResult> SoftDeleteReviewAsync(Guid reviewId)
    {
        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null)
            return OperationResult.ResourceNotFound(nameof(review), reviewId);
        var deleteResult = review.SoftDelete();
        if(!deleteResult)
            return OperationResult.Success(SuccessTypes.NotModified);

        await _reviewRepository.UpdateAsync(review);
      

        return OperationResult.Success();
    }



   public async Task<OperationResult<(List<ReviewDto> Reviews, int TotalCount)>> GetReviewsByCourseIdAsync(Guid courseId, int? page = null, int? pageSize = null)
    {
       var result = await _reviewRepository.GetReviewsByCourseIdAsync(courseId, page, pageSize);
       if (result.Reviews is null || !result.Reviews.Any())
       { 
           return OperationResult<(List<ReviewDto>, int TotalCount)>.ResourceNotFound("reviews for this course", courseId);
       }
       
       return OperationResult<(List<ReviewDto>, int TotalCount)>.Success(result);
        
    }
    public async Task<OperationResult<(List<ReviewDto> Reviews, int TotalCount)>> GetReviewsByVideoIdAsync(Guid videoId, int? page = null, int? pageSize = null)
    {
        var result = await _reviewRepository.GetReviewsByVideoIdAsync(videoId, page, pageSize);
        if (result.Reviews is null || !result.Reviews.Any())
        { 
            return OperationResult<(List<ReviewDto>, int TotalCount)>.ResourceNotFound("reviews for this video", videoId);
        }
       
        return OperationResult<(List<ReviewDto>, int TotalCount)>.Success(result);
        
    }
}
