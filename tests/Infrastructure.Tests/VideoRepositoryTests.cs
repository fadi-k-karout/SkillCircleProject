using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Infrastructure.Tests;

public class VideoRepositoryTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ApplicationDbContext _context;
    private readonly VideoRepository _videoRepository;

    public VideoRepositoryTests(ITestOutputHelper testOutputHelper)
    {
       _testOutputHelper = testOutputHelper;
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _videoRepository = new VideoRepository(_context); 
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnVideo_WhenVideoExists()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var video = new Video{
            Id = videoId,
            Title= "Video Title",
            Description= "Video Description",
            Slug= "video-slug",
            CourseId= Guid.NewGuid(),
            CreatorId= Guid.NewGuid(),
            ProviderVideoId = $"providerVideo-id",
            ProviderName = "Provider Name"
        };
        
        await _videoRepository.AddAsync(video);

        // Act
        var result = await _videoRepository.GetByIdAsync(videoId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(videoId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenVideoDoesNotExist()
    {
        // Act
        var result = await _videoRepository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddVideoToDatabase()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var video = new Video{
            Id = videoId,
            Title= "New Video",
            Description= "Description",
            Slug= "new-video",
            CourseId= Guid.NewGuid(),
            CreatorId= Guid.NewGuid(),
            ProviderVideoId = $"providerVideo-id",
            ProviderName = "Provider Name"
        };

        // Act
        await _videoRepository.AddAsync(video);
        var result = await _context.Videos.FindAsync(videoId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(video.Title, result.Title);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateVideoDetails()
    {
        // Arrange
        var videoId = Guid.NewGuid();
        var video = new Video{
            Id = videoId,
            Title= "Original Title",
            Description= "Original Description",
            Slug= "original-slug",
            CourseId= Guid.NewGuid(),
            CreatorId= Guid.NewGuid(),
            ProviderVideoId = $"providerVideo-id",
            ProviderName = "Provider Name"
        };
         _context.Videos.Add(video);
         await _context.SaveChangesAsync();

        // Update video details
        video.Title = "Updated Title";
        video.Description = "Updated Description";

        // Act
        await _videoRepository.UpdateAsync(video);
        
        var updatedVideo = await _context.Videos.FindAsync(videoId);

        // Assert
        Assert.NotNull(updatedVideo);
        Assert.Equal("Updated Title", updatedVideo.Title);
        Assert.Equal("Updated Description", updatedVideo.Description);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveVideoFromDatabase()
    {
        // Arrange
        var videoId = Guid.NewGuid();
      
        var video = new Video{
            Id = videoId,
            Title= "Video to Delete",
            Description= "Video Description",
            Slug= "delete-video",
            CourseId= Guid.NewGuid(),
            CreatorId= Guid.NewGuid(),
            ProviderVideoId = $"providerVideo-id",
            ProviderName = "Provider Name"
        };
        await _videoRepository.AddAsync(video);

        // Act
        await _videoRepository.DeleteAsync(videoId);
        var result = await _context.Videos.FindAsync(videoId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPaginatedByCourseIdAsync_ShouldReturnPaginatedVideos_WhenCourseHasVideos()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        for (int i = 0; i < 5; i++)
        {
            var video = new Video
            {
                Id = Guid.NewGuid(),
                Title = $"Video {i + 1}",
                Description = $"Description {i + 1}",
                Slug = $"video-slug-{i + 1}",
                CourseId = courseId,
                CreatorId = Guid.NewGuid(),
                ProviderVideoId = $"providerVideo-id{i + 1}",
                ProviderName = "Provider Name"
            };
            await _videoRepository.AddAsync(video);
        }

        // Act
        var (videos, totalCount) = await _videoRepository.GetPaginatedByCourseIdAsync(courseId, page: 1, pageSize: 3);
       
        
        // Assert
        Assert.Equal(3, videos.Count);
        Assert.Equal(5, totalCount);
    }

    [Fact]
    public async Task GetVideosByCourseIdAsync_ShouldReturnVideos_WhenCourseHasVideos()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        for (int i = 0; i < 3; i++)
        {
            var video = new Video
            {
                Id = Guid.NewGuid(),
                Title = $"Video {i + 1}",
                Description = $"Description {i + 1}",
                Slug = $"video-slug-{i + 1}",
                CourseId = courseId,
                CreatorId = Guid.NewGuid(),
                ProviderVideoId = $"providerVideo-id{i + 1}",
                ProviderName = "Provider Name"
            };
            await _videoRepository.AddAsync(video);
        }

        
       

        // Act
        var videos = await _videoRepository.GetVideosByCourseIdAsync(courseId);

        // Assert
        Assert.Equal(3, videos.Count);
    }

    [Fact]
    public async Task GetOwnerIdAsync_ShouldReturnCreatorId_WhenVideoExists()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var videoId = Guid.NewGuid();
        var video = new Video
        {
            Id = videoId,
            Title = "Owner Video",
            Description = "Description",
            Slug = "owner-video",
            CourseId = Guid.NewGuid(),
            CreatorId = creatorId,
            ProviderVideoId = $"providerVideo-id",
            ProviderName = "Provider Name"

        };
          
        await _videoRepository.AddAsync(video);

        // Act
        var result = await _videoRepository.GetOwnerIdAsync(videoId);

        // Assert
        Assert.Equal(creatorId.ToString(), result);
    }

    [Fact]
    public async Task GetOwnerIdAsync_ShouldReturnNull_WhenVideoDoesNotExist()
    {
        // Act
        var result = await _videoRepository.GetOwnerIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }
}
