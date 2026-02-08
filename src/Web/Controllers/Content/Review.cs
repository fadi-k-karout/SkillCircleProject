using Application.Authorization;
using Application.Common.Interfaces.Content;
using Application.Common.Interfaces.repos;
using Application.DTOs;
using Application.DTOs.Content;
using Microsoft.AspNetCore.Authorization;
using Web.Extensions;

namespace Web.Controllers.Content;

using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

// Assuming the necessary namespaces are included
[ApiController]
[Route("api/reviews")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IReviewRepository _reviewRepository;
    
    private const string CanManageReviews = PolicyName.CanManageReviews;
    private const string Admin = RoleName.Admin;

    public ReviewController(IReviewService reviewService, IAuthorizationService authorizationService, IReviewRepository reviewRepository)
    {
        _reviewService = reviewService;
        _authorizationService = authorizationService;
        _reviewRepository = reviewRepository;
    }

    #region Add Review

    /// <summary>
    /// Adds a new review for a course.
    /// </summary>
    /// <param name="reviewDto">The review data transfer object containing review details.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    [HttpPost]
    public async Task<IActionResult> AddReviewAsync([FromBody] ReviewCreateUpdateDto reviewDto)
    {
        var result = await _reviewService.CreateReviewAsync(reviewDto);
        return result.ToActionResult();
    }

    #endregion

    #region Update Review

    /// <summary>
    /// Updates an existing review.
    /// </summary>
    /// <param name="reviewId">The ID of the review to update.</param>
    /// <param name="reviewDto">The review data transfer object containing updated review details.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    [HttpPut("{reviewId}")]
    public async Task<IActionResult> UpdateReviewAsync(Guid reviewId, [FromBody] ReviewCreateUpdateDto reviewDto)
    {
        var ownerId = _reviewRepository.GetOwnerIdAsync(reviewId);
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, ownerId,CanManageReviews );
        
        if (!authorizationResult.Succeeded)
        {
            return new ForbidResult();
        }
        var result = await _reviewService.UpdateReviewAsync(reviewId, reviewDto);
        return result.ToActionResult();
    }

    #endregion

    #region Delete Review

    /// <summary>
    /// Deletes a review by its ID.
    /// </summary>
    /// <param name="reviewId">The ID of the review to delete.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    [Authorize]
    [HttpDelete("{reviewId}")]
    public async Task<IActionResult> DeleteReviewAsync(Guid reviewId)
    {
        var ownerId = _reviewRepository.GetOwnerIdAsync(reviewId);
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, ownerId,CanManageReviews );
        
        if (!authorizationResult.Succeeded)
        {
            return new ForbidResult();
        }

        var result = await _reviewService.SoftDeleteReviewAsync(reviewId);
        return result.ToActionResult();
    }

    #endregion

    #region Soft Delete Review

    /// <summary>
    /// Soft deletes a review by its ID.
    /// </summary>
    /// <param name="reviewId">The ID of the review to soft delete.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    [HttpPatch("{reviewId}/softdelete")]
    public async Task<IActionResult> SoftDeleteReviewAsync(Guid reviewId)
    {
        var ownerId = _reviewRepository.GetOwnerIdAsync(reviewId);
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, ownerId,CanManageReviews );
        
        if (!authorizationResult.Succeeded)
        {
            return new ForbidResult();
        }
        var result = await _reviewService.SoftDeleteReviewAsync(reviewId);
        return result.ToActionResult();
    }

    #endregion

    #region Get Reviews By Course ID

    /// <summary>
    /// Retrieves reviews for a specific course.
    /// </summary>
    /// <param name="courseId">The ID of the course to retrieve reviews for.</param>
 
    /// <returns>A collection of reviews for the specified course.</returns>
    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetReviewsByCourseIdAsync(Guid courseId, [FromQuery] PaginationQuery paginationQuery)
    {
        var result = await _reviewService.GetReviewsByCourseIdAsync(courseId, paginationQuery.Page.Value, paginationQuery.PageSize.Value);
        return result.ToActionResult();
    }

    #endregion
    
    #region Get Reviews By Video ID

    /// <summary>
    /// Retrieves reviews for a specific Video.
    /// </summary>
    /// <param name="courseId">The ID of the Video to retrieve reviews for.</param>
 
    /// <returns>A collection of reviews for the specified Video.</returns>
    [HttpGet("videos/{courseId}")]
    public async Task<IActionResult> GetReviewsByVideoIdAsync(Guid videoId, [FromQuery] PaginationQuery paginationQuery)
    {
        var result = await _reviewService.GetReviewsByVideoIdAsync(videoId, paginationQuery.Page.Value, paginationQuery.PageSize.Value);
        return result.ToActionResult();
    }

    #endregion
}
