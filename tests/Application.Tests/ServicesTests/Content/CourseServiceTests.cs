using Application.Common.Interfaces.repos;
using Application.DTOs.Content;
using Application.Services.Content;
using Domain.Models;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace Application.Tests.ServicesTests.Content;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;

public class CourseServiceTests
{
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly Mock<IVideoRepository> _videoRepositoryMock;
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CourseService _courseService;

    public CourseServiceTests()
    {
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _videoRepositoryMock = new Mock<IVideoRepository>();
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        
        _courseService = new CourseService(
            _courseRepositoryMock.Object, 
            _videoRepositoryMock.Object, 
            _reviewRepositoryMock.Object, 
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task GetCourseWithVideosAsync_ShouldReturnCourse_WhenCourseExists()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var courseDto = new CourseWithVideosDto();
        _courseRepositoryMock.Setup(repo => repo.GetCourseWithVideosAsync(courseId))
            .ReturnsAsync(courseDto);

        // Act
        var result = await _courseService.GetCourseWithVideosAsync(courseId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(courseDto, result.Value);
    }

    [Fact]
    public async Task GetCourseWithVideosAsync_ShouldReturnNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _courseRepositoryMock.Setup(repo => repo.GetCourseWithVideosAsync(courseId))
            .ReturnsAsync((CourseWithVideosDto)null);

        // Act
        var result = await _courseService.GetCourseWithVideosAsync(courseId);

        // Assert
        Assert.False(result.IsSuccess);
      
    }

    [Fact]
    public async Task CreateCourseAsync_ShouldCreateCourse_WhenValidDtoIsProvided()
    {
        // Arrange
        var courseDto = new CourseCreateUpdateDto
        {
            Title = "Test Course",
            Description = "Description",
            SkillId = Guid.NewGuid(),
            CreatorId = Guid.NewGuid(),
            IsPaid = true
        };

        // Act
        var result = await _courseService.CreateCourseAsync(courseDto);

        // Assert
        Assert.True(result.IsSuccess);
        _courseRepositoryMock.Verify(repo => repo.AddWithNoSaveAsync(It.IsAny<Course>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateCourseAsync_ShouldReturnArgumentNullError_WhenDtoIsNull()
    {
        // Act
        var result = await _courseService.CreateCourseAsync(null);

        // Assert
        Assert.False(result.IsSuccess);
       
    }

    [Fact]
    public async Task UpdateCourseAsync_ShouldUpdateCourse_WhenCourseExists()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var courseDto = new CourseCreateUpdateDto { Title = "Updated Title" };
        var existingCourse =  new Course
        {
            Title = "Old Title",
            Description = "Old Description",
            Slug = "old-slug",
            CreatorId = Guid.NewGuid(),
            IsPaid = true,
            IsPrivate = false,
            Status = new Status { CreatedAt = DateTime.UtcNow }
        };
        
        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId)).ReturnsAsync(existingCourse);

        // Act
        var result = await _courseService.UpdateCourseAsync(courseId, courseDto);

        // Assert
        Assert.True(result.IsSuccess);
        _courseRepositoryMock.Verify(repo => repo.UpdateAsync(existingCourse), Times.Once);
    }

    [Fact]
    public async Task UpdateCourseAsync_ShouldReturnNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId)).ReturnsAsync((Course)null);

        // Act
        var result = await _courseService.UpdateCourseAsync(courseId, new CourseCreateUpdateDto());

        // Assert
        Assert.False(result.IsSuccess);
        
    }

    [Fact]
    public async Task DeleteCourseAsync_ShouldSoftDeleteCourse_WhenCourseExists()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var status = new Status{IsSoftDeleted = false};
        var course = new Course
        {
            Title = "Course",
            Description = "Description",
            Slug = "course",
            CreatorId = Guid.NewGuid(),
            IsPaid = true,
            IsPrivate = false,
            Status = new Status { CreatedAt = DateTime.UtcNow }
        };
        course.Status = status;
        course.Id = courseId;
        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId)).ReturnsAsync(course);
        _videoRepositoryMock.Setup(repo => repo.GetVideosByCourseIdAsync(courseId)).ReturnsAsync(new List<Video>());

        // Act
        var result = await _courseService.DeleteCourseAsync(courseId);

        // Assert
        Assert.True(result.IsSuccess);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task MakeCoursePaidAsync_ShouldUpdateCourseAndVideos_WhenCourseExists()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Title = "New Course",
            Description = "Description",
            Slug = "new-course",
            CreatorId = Guid.NewGuid(),
            IsPaid = false,
            IsPrivate = false,
            Status = new Status { CreatedAt = DateTime.UtcNow }
        };
        var videos = new List<Video>
        { 
            new Video
            {
            Id = Guid.NewGuid(),
            Title = "video title",
            Slug = "video-title",
            Description = "Video Description",
            CourseId = Guid.NewGuid(),
            CreatorId =  Guid.NewGuid(),
            ProviderVideoId = "videoid",
            ProviderName = "providername",
            IsPaid = false,
            }
        };
        
        
        _courseRepositoryMock.Setup(repo => repo.GetByIdAsync(courseId)).ReturnsAsync(course);
        _videoRepositoryMock.Setup(repo => repo.GetVideosByCourseIdAsync(courseId)).ReturnsAsync(videos);

        // Act
        var result = await _courseService.MakeCoursePaidAsync(courseId);

        // Assert
        Assert.True(result.IsSuccess);
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
       
    }



   
}
