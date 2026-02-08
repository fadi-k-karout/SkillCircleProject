using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repositories;

namespace Infrastructure.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class CourseRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly CourseRepository _courseRepository;
    
    private Guid _userId = Guid.NewGuid();

    public CourseRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique database for each test
            .Options;

        _context = new ApplicationDbContext(options);
        _courseRepository = new CourseRepository(_context);
        _context.Users.Add(new User
        {
            Id = _userId,
            FirstName = "John",
            IsActive = true

        });
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCourse_WhenCourseExists()
    {
        // Arrange
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Test Course",
            Description = "Description",
            Slug = "test-course",
            CreatorId = Guid.NewGuid(),
            IsPaid = true,
            IsPrivate = false
        };
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        // Act
        var result = await _courseRepository.GetByIdAsync(course.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(course.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCourseDoesNotExist()
    {
        // Act
        var result = await _courseRepository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddCourseToDatabase()
    {
        // Arrange
        var course = new Course
        {
            Title = "New Course",
            Description = "Description",
            Slug = "new-course",
            CreatorId = Guid.NewGuid(),
            IsPaid = true,
            IsPrivate = false,
            Status = new Status { CreatedAt = DateTime.UtcNow }
        };

        // Act
        await _courseRepository.AddAsync(course);
        var result = await _context.Courses.FindAsync(course.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(course.Title, result.Title);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCourseDetails()
    {
        // Arrange
        var course = new Course
        {
            Title = "Original Title",
            Description = "Original Description",
            Slug = "original-slug",
            CreatorId = Guid.NewGuid(),
            IsPaid = true,
            IsPrivate = false
        };
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        // Update course details
        course.Title = "Updated Title";
        course.Description = "Updated Description";

        // Act
        await _courseRepository.UpdateAsync(course);
        var updatedCourse = await _context.Courses.FindAsync(course.Id);

        // Assert
        Assert.NotNull(updatedCourse);
        Assert.Equal("Updated Title", updatedCourse.Title);
        Assert.Equal("Updated Description", updatedCourse.Description);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCourseFromDatabase()
    {
        // Arrange
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Course to Delete",
            Description = "Description",
            Slug = "delete-course",
            CreatorId = Guid.NewGuid(),
            IsPaid = true,
            IsPrivate = false
        };
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        // Act
        await _courseRepository.DeleteAsync(course.Id);
        var result = await _context.Courses.FindAsync(course.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetCourseWithVideosAsync_ShouldReturnCourseWithVideos_WhenCourseExists()
    {
        var courseId = Guid.NewGuid();
        var creatorId= _userId;
        // Arrange
        var course = new Course
        {
            Id = courseId,
            Title = "Course with Videos",
            Description = "Description",
            Slug = "course-with-videos",
            CreatorId = _userId,
            IsPaid = true,
            IsPrivate = false
        };

        var video = new Video
        {
            Title = "Video Title",
            Description = "Video Description",
            Slug = "video-slug",
            CourseId = courseId,
            CreatorId = _userId,
            ProviderVideoId = $"providerVideo-id",
            ProviderName = "Provider Name"
        };

        course.Videos.Add(video);
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        // Act
        var result = await _courseRepository.GetCourseWithVideosAsync(courseId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(course.Id, result.Course.Id);
        Assert.Single(result.Videos);
    }

    [Fact]
    public async Task GetCourseWithPaginatedVideosAsync_ShouldReturnPaginatedVideos()
    {
        var courseId = Guid.NewGuid();
        var creatorId = _userId;
        // Arrange
        var course = new Course
        {
            Id = courseId,
            Title = "Course with Multiple Videos",
            Description = "Description",
            Slug = "multiple-videos",
            CreatorId = creatorId,
            IsPaid = true,
            IsPrivate = false
        };

        for (int i = 0; i < 5; i++)
        {
            course.Videos.Add(new Video
            {
                Id = Guid.NewGuid(),
                Title = $"Video {i + 1}",
                Description = $"description-{i + 1}",
                Slug = $"video-{i + 1}",
                CourseId = courseId,
                CreatorId = creatorId,
                ProviderVideoId = $"providerVideo-id{i + 1}",
                ProviderName = "Provider Name"
            });
        }
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        // Act
        var result = await _courseRepository.GetCourseWithPaginatedVideosAsync(courseId, page: 1, pageSize: 2);
        result.Videos = result.Videos.OrderBy(video => video.Title).ToList();
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Videos.Count);
        Assert.Equal("Video 4", result.Videos.First().Title);
        Assert.Equal("Video 5", result.Videos.Last().Title);
    }

    [Fact]
    public async Task GetCoursesDetailsByCreatorIdAsync_ShouldReturnCourses_WhenCoursesExistForCreator()
    {
        // Arrange
        var creatorId = _userId;
        var course1 = new Course
        {
            Title = "Creator Course 1",
            Description = "Description 1",
            Slug = "course-1",
            CreatorId = creatorId,
            IsPaid = true,
            IsPrivate = false
        };

        var course2 = new Course
        {
            Title = "Creator Course 2",
            Description = "Description 2",
            Slug = "course-2",
            CreatorId = creatorId,
            IsPaid = true,
            IsPrivate = false
        };

        _context.Courses.AddRange(course1, course2);
        await _context.SaveChangesAsync();

        // Act
        var (courses, totalCount) = await _courseRepository.GetCoursesDetailsByCreatorIdAsync(creatorId, null, null);

        // Assert
        Assert.Equal(5, totalCount);
        Assert.Equal(2, courses.Count);
        Assert.NotNull(courses);
        Assert.NotNull(courses[0].Creator);
        Assert.NotNull(courses[1].Creator);
        Assert.Contains(courses, c => c.Title == "Creator Course 1");
        Assert.Contains(courses, c => c.Title == "Creator Course 2");
    }

    [Fact]
    public async Task GetOwnerIdAsync_ShouldReturnCreatorId_WhenCourseExists()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var course = new Course
        {
            Title = "Course with Owner",
            Description = "Description",
            Slug = "owner-course",
            CreatorId = creatorId,
            IsPaid = true,
            IsPrivate = false
        };
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        // Act
        var result = await _courseRepository.GetOwnerIdAsync(course.Id);

        // Assert
        Assert.Equal(creatorId.ToString(), result);
    }

    [Fact]
    public async Task GetOwnerIdAsync_ShouldReturnNull_WhenCourseDoesNotExist()
    {
        // Act
        var result = await _courseRepository.GetOwnerIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }
}