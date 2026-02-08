using System.Buffers;
using Application.Common.Interfaces.repos;
using Application.Common.Operation;
using Application.DTOs.Content;
using Application.Services.Content;
using Domain.Models;

namespace Application.Tests.ServicesTests.Content;

using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class VideoServiceTests
{
    private readonly Mock<IVideoRepository> _mockVideoRepository;
    private readonly VideoService _videoService;

    public VideoServiceTests()
    {
        _mockVideoRepository = new Mock<IVideoRepository>();
        _videoService = new VideoService(_mockVideoRepository.Object);
    }



[Fact]
public async Task GetVideoByIdAsync_ReturnsVideoDto_WhenVideoExists()
{
    // Arrange
    var videoId = Guid.NewGuid();
    var video = new Video 
        { Id = videoId,
            Title = "Test Video",
            Slug = "test-video",
            Description = "Test Description",
            CourseId = Guid.NewGuid(),
            CreatorId =  Guid.NewGuid(),
            ProviderVideoId = "videoid",
            ProviderName = "providername",
            
            
        };
    
    _mockVideoRepository
        .Setup(repo => repo.GetByIdAsync(videoId))
        .ReturnsAsync(video); // Make sure the return type is clear

    // Act
    var result = await _videoService.GetVideoByIdAsync(videoId);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    Assert.Equal(video.Title, result.Value.Title);
}

[Fact]
public async Task GetVideoByIdAsync_ReturnsNotFound_WhenVideoDoesNotExist()
{
    // Arrange
    var videoId = Guid.NewGuid();
    _mockVideoRepository
        .Setup(repo => repo.GetByIdAsync(videoId))
        .ReturnsAsync((Video)null); // Explicitly returning null

    // Act
    var result = await _videoService.GetVideoByIdAsync(videoId);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Equal($"Video with ID {videoId} was not found.", result.ErrorMessage);
}

[Fact]
public async Task CreateVideoAsync_ReturnsSuccess_WhenVideoIsCreated()
{
    // Arrange
    var videoDto = new VideoCreateUpdateDto {
        Title = "New Video",
        Description = "New Description"
        
    };
    var video = new Video
    {
        Id = Guid.NewGuid(),
        Title = videoDto.Title,
        Slug = "new-video",
        Description = videoDto.Description,
        CourseId = Guid.NewGuid(),
        CreatorId =  Guid.NewGuid(),
        ProviderVideoId = "videoid",
        ProviderName = "providername",
    };

    _mockVideoRepository
        .Setup(repo => repo.AddAsync(It.IsAny<Video>()));
         // Ensure the return type matches

    // Act
    var result = await _videoService.CreateVideoAsync(videoDto);

    // Assert
    Assert.True(result.IsSuccess);
    
}

[Fact]
public async Task UpdateVideoAsync_ReturnsSuccess_WhenVideoIsUpdated()
{
    // Arrange
    var videoId = Guid.NewGuid();
    var existingVideo =  new Video
    {
        Id = Guid.NewGuid(),
        Title = "Old Title",
        Slug = "old-title",
        Description = "Old Description",
        CourseId = Guid.NewGuid(),
        CreatorId =  Guid.NewGuid(),
        ProviderVideoId = "videoid",
        ProviderName = "providername",
    };
    var updatedVideoDto = new VideoCreateUpdateDto { Title = "Updated Title", Description = "Updated Description" };

    _mockVideoRepository
        .Setup(repo => repo.GetByIdAsync(videoId))
        .ReturnsAsync(existingVideo);
    _mockVideoRepository
        .Setup(repo => repo.UpdateAsync(It.IsAny<Video>()))
        .Returns(Task.CompletedTask);

    // Act
    var result = await _videoService.UpdateVideoAsync(videoId, updatedVideoDto);

    // Assert
    Assert.True(result.IsSuccess);
    _mockVideoRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Video>()), Times.Once);
}

[Fact]
public async Task DeleteVideoAsync_ReturnsSuccess_WhenVideoIsDeleted()
{
    // Arrange
    var videoId = Guid.NewGuid();
    var video = new Video
    {
        Id = Guid.NewGuid(),
        Title = "video title",
        Slug = "video-title",
        Description = "Video Description",
        CourseId = Guid.NewGuid(),
        CreatorId =  Guid.NewGuid(),
        ProviderVideoId = "videoid",
        ProviderName = "providername",
    };
    _mockVideoRepository
        .Setup(repo => repo.GetByIdAsync(videoId))
        .ReturnsAsync(video);
    _mockVideoRepository
        .Setup(repo => repo.DeleteAsync(videoId))
        .Returns(Task.CompletedTask);

    // Act
    var result = await _videoService.DeleteVideoAsync(videoId);

    // Assert
    Assert.True(result.IsSuccess);
    _mockVideoRepository.Verify(repo => repo.DeleteAsync(videoId), Times.Once);
}

[Fact]
public async Task DeleteVideoAsync_ReturnsNotFound_WhenVideoDoesNotExist()
{
    // Arrange
    var videoId = Guid.NewGuid();
    _mockVideoRepository
        .Setup(repo => repo.GetByIdAsync(videoId))
        .ReturnsAsync((Video)null);

    // Act
    var result = await _videoService.DeleteVideoAsync(videoId);

    // Assert
    Assert.False(result.IsSuccess);
    Assert.Equal($"Video with ID {videoId} was not found.", result.ErrorMessage);
}
}

