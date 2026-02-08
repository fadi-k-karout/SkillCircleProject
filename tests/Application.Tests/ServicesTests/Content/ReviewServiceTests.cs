using Application.Common.Interfaces.repos;
using Application.Common.Operation;
using Application.DTOs.Content;
using Application.Services.Content;
using Domain.Models;

namespace Application.Tests.ServicesTests.Content;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

public class ReviewServiceTests
{
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ReviewService _reviewService;

    public ReviewServiceTests()
    {
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _reviewService = new ReviewService(_reviewRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateReviewAsync_ShouldReturnSuccess_WhenValidReviewDtoIsProvided()
    {
        // Arrange
        var reviewDto = new ReviewCreateUpdateDto
        {
            CourseId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Rating = 5,
            Content = "Great course!"
        };

        // Act
        var result = await _reviewService.CreateReviewAsync(reviewDto);

        // Assert
        Assert.True(result.IsSuccess);
        _reviewRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Review>()), Times.Once);
    }

    [Fact]
    public async Task CreateReviewAsync_ShouldReturnArgumentNullError_WhenReviewDtoIsNull()
    {
        // Act
        var result = await _reviewService.CreateReviewAsync(null);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task UpdateReviewAsync_ShouldReturnSuccess_WhenReviewExists()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var reviewDto = new ReviewCreateUpdateDto { Rating = 4, Content = "Updated review content" };
        var existingReview = new Review
        {
            Id = reviewId,
            Rating = 5,
            Content = "Old review content",
            Status = new Status { CreatedAt = DateTime.UtcNow }
        };

        _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(reviewId)).ReturnsAsync(existingReview);

        // Act
        var result = await _reviewService.UpdateReviewAsync(reviewId, reviewDto);

        // Assert
        Assert.True(result.IsSuccess);
        _reviewRepositoryMock.Verify(repo => repo.UpdateAsync(existingReview), Times.Once);
    }

    [Fact]
    public async Task UpdateReviewAsync_ShouldReturnNotFound_WhenReviewDoesNotExist()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(reviewId)).ReturnsAsync((Review)null);

        // Act
        var result = await _reviewService.UpdateReviewAsync(reviewId, new ReviewCreateUpdateDto());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task DeleteReviewAsync_ShouldReturnSuccess_WhenReviewExists()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var existingReview = new Review { Id = reviewId };

        _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(reviewId)).ReturnsAsync(existingReview);

        // Act
        var result = await _reviewService.DeleteReviewAsync(reviewId);

        // Assert
        Assert.True(result.IsSuccess);
        _reviewRepositoryMock.Verify(repo => repo.DeleteAsync(reviewId), Times.Once);
    }

    [Fact]
    public async Task DeleteReviewAsync_ShouldReturnNotFound_WhenReviewDoesNotExist()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(reviewId)).ReturnsAsync((Review)null);

        // Act
        var result = await _reviewService.DeleteReviewAsync(reviewId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task SoftDeleteReviewAsync_ShouldReturnSuccess_WhenReviewExists()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var existingReview = new Review
        {
            Id = reviewId,
            Status = new Status { IsSoftDeleted = false }
        };

        _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(reviewId)).ReturnsAsync(existingReview);

        // Act
        var result = await _reviewService.SoftDeleteReviewAsync(reviewId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(existingReview.Status.IsSoftDeleted);
        _reviewRepositoryMock.Verify(repo => repo.UpdateAsync(existingReview), Times.Once);
    }

    [Fact]
    public async Task SoftDeleteReviewAsync_ShouldReturnNotModified_WhenReviewIsAlreadySoftDeleted()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var status = new Status { IsSoftDeleted = true };
        var existingReview = new Review
        {
            Status = status,
        };

        _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(reviewId)).ReturnsAsync(existingReview);

        // Act
        var result = await _reviewService.SoftDeleteReviewAsync(reviewId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(SuccessTypes.NotModified, result.SuccessType);
        _reviewRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Review>()), Times.Never);
    }

    [Fact]
    public async Task GetReviewsByCourseIdAsync_ShouldReturnReviews_WhenReviewsExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var reviews = new List<ReviewDto> 
        {
            new ReviewDto { Content = "Review 1", Rating = 5 },
            new ReviewDto { Content = "Review 2", Rating = 4 }
        };
        var totalCount = reviews.Count;

        _reviewRepositoryMock.Setup(repo => repo.GetReviewsByCourseIdAsync(courseId, 1, 10))
            .ReturnsAsync((reviews, totalCount));

        // Act
        var result = await _reviewService.GetReviewsByCourseIdAsync(courseId, 1, 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(totalCount, result.Value.TotalCount);
        Assert.Collection(result.Value.Reviews,
            item => Assert.Equal("Review 1", item.Content),
            item => Assert.Equal("Review 2", item.Content)
        );
    }

    [Fact]
    public async Task GetReviewsByCourseIdAsync_ShouldReturnNotFound_WhenNoReviewsExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _reviewRepositoryMock.Setup(repo => repo.GetReviewsByCourseIdAsync(courseId, 1, 10))
            .ReturnsAsync((new List<ReviewDto>(), 0));

        // Act
        var result = await _reviewService.GetReviewsByCourseIdAsync(courseId, 1, 10);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }
}
