using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Infrastructure.Tests;

public class ReviewRepositoryTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ApplicationDbContext _context;
    private readonly ReviewRepository _reviewRepository;
    
    private Guid _userId = Guid.NewGuid();

    public ReviewRepositoryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _context.Users.Add(new User
        {
            Id = _userId,
            FirstName = "John",
            IsActive = true

        });
        _reviewRepository = new ReviewRepository(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnReview_WhenReviewExists()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var review = new Review("Great course!", 5, Guid.NewGuid(), Guid.NewGuid()) { Id = reviewId };
        await _reviewRepository.AddAsync(review);

        // Act
        var result = await _reviewRepository.GetByIdAsync(reviewId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reviewId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenReviewDoesNotExist()
    {
        // Act
        var result = await _reviewRepository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddReviewToDatabase()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var review = new Review("Excellent course!", 5, Guid.NewGuid(), Guid.NewGuid()) { Id = reviewId };

        // Act
        await _reviewRepository.AddAsync(review);
        var result = await _context.Reviews.FindAsync(reviewId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(review.Rating, result.Rating);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateReviewDetails()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var review = new Review("Needs improvement", 3, Guid.NewGuid(), Guid.NewGuid()) { Id = reviewId };
        await _reviewRepository.AddAsync(review);

        // Update review details
        review.Content = "Much better!";
        review.Rating = 4;

        // Act
        await _reviewRepository.UpdateAsync(review);
        var updatedReview = await _context.Reviews.FindAsync(reviewId);

        // Assert
        Assert.NotNull(updatedReview);
        Assert.Equal("Much better!", updatedReview.Content);
        Assert.Equal(4, updatedReview.Rating);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveReviewFromDatabase()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var review = new Review("To be deleted", 1, Guid.NewGuid(), Guid.NewGuid()) { Id = reviewId };
        await _reviewRepository.AddAsync(review);

        // Act
        await _reviewRepository.DeleteAsync(reviewId);
        var result = await _context.Reviews.FindAsync(reviewId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetReviewsByCourseIdAsync_ShouldReturnReviews_WhenCourseHasReviews()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        for (int i = 0; i < 5; i++)
        {
            var review = new Review($"Review {i + 1}", i + 1, courseId,_userId);
            await _reviewRepository.AddAsync(review);
        }

        // Act
        var (reviews, totalCount) = await _reviewRepository.GetReviewsByCourseIdAsync(courseId, page: 1, pageSize: 3);
        foreach (var review in reviews)
        {
            Assert.NotNull(review);
            Assert.NotNull(review.User);
            _testOutputHelper.WriteLine($"{review.Id}: {review.Content}\n {review.User.FirstName} {review.User.LastName}");
        }
        // Assert
        Assert.NotNull(reviews);
        Assert.Equal(3, reviews.Count);
        Assert.Equal(5, totalCount);
    }

    [Fact]
    public async Task GetOwnerIdAsync_ShouldReturnUserId_WhenReviewExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var reviewId = Guid.NewGuid();
        var review = new Review("Owner Review", 5,  Guid.NewGuid(), userId) { Id = reviewId };
        await _reviewRepository.AddAsync(review);

        // Act
        var result = await _reviewRepository.GetOwnerIdAsync(reviewId);

        // Assert
        Assert.Equal(userId.ToString(), result);
    }

    [Fact]
    public async Task GetOwnerIdAsync_ShouldReturnNull_WhenReviewDoesNotExist()
    {
        // Act
        var result = await _reviewRepository.GetOwnerIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }
    
}
