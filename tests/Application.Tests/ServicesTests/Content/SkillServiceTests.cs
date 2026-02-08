using Application.Common.Interfaces.repos;
using Application.Common.Operation;
using Application.DTOs.Content;
using Application.Services.Content;
using Domain.Models;
using Moq;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

namespace Application.Tests.ServicesTests.Content;


public class SkillServiceTests
{
    private readonly Mock<ISkillRepository> _skillRepositoryMock;
    private readonly SkillService _skillService;

    public SkillServiceTests()
    {
        _skillRepositoryMock = new Mock<ISkillRepository>();
        _skillService = new SkillService(_skillRepositoryMock.Object);
    }

    [Fact]
    public async Task GetSkillWithCoursesById_ShouldReturnSkill_WhenSkillExists()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var skillWithCoursesDto = new SkillWithCoursesDto();
        _skillRepositoryMock.Setup(repo => repo.GetSkillWithCoursesByIdAsync(skillId))
            .ReturnsAsync(skillWithCoursesDto);

        // Act
        var result = await _skillService.GetSkillWithCoursesById(skillId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(skillWithCoursesDto, result.Value);
    }

    [Fact]
    public async Task GetSkillWithCoursesById_ShouldReturnNotFound_WhenSkillDoesNotExist()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        _skillRepositoryMock.Setup(repo => repo.GetSkillWithCoursesByIdAsync(skillId))
            .ReturnsAsync((SkillWithCoursesDto)null);

        // Act
        var result = await _skillService.GetSkillWithCoursesById(skillId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task CreateSkill_ShouldReturnSuccess_WhenValidSkillDtoProvided()
    {
        // Arrange
        var skillDto = new SkillDto { Name = "New Skill", Description = "Skill Description" };

        // Act
        var result = await _skillService.CreateSkill(skillDto);

        // Assert
        Assert.True(result.IsSuccess);
        _skillRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Skill>()), Times.Once);
    }

    [Fact]
    public async Task CreateSkill_ShouldReturnArgumentNullError_WhenSkillDtoIsNull()
    {
        // Act
        var result = await _skillService.CreateSkill(null);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task UpdateSkill_ShouldReturnSuccess_WhenSkillExists()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var skillDto = new SkillDto { Name = "Updated Skill", Description = "Updated Description" };
        var existingSkill = new Skill("Old Skill", "Old Description", "old-slug", new Status());
        
        _skillRepositoryMock.Setup(repo => repo.GetByIdAsync(skillId)).ReturnsAsync(existingSkill);

        // Act
        var result = await _skillService.UpdateSkill(skillId, skillDto);

        // Assert
        Assert.True(result.IsSuccess);
        _skillRepositoryMock.Verify(repo => repo.UpdateAsync(existingSkill), Times.Once);
    }

    [Fact]
    public async Task UpdateSkill_ShouldReturnNotFound_WhenSkillDoesNotExist()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        _skillRepositoryMock.Setup(repo => repo.GetByIdAsync(skillId)).ReturnsAsync((Skill)null);

        // Act
        var result = await _skillService.UpdateSkill(skillId, new SkillDto());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task SoftDeleteSkill_ShouldReturnSuccess_WhenSkillExists()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var skill = new Skill("Skill", "Description", "slug", new Status());
        _skillRepositoryMock.Setup(repo => repo.GetByIdAsync(skillId)).ReturnsAsync(skill);

        // Act
        var result = await _skillService.SoftDeleteSkill(skillId);

        // Assert
        Assert.True(result.IsSuccess);
        _skillRepositoryMock.Verify(repo => repo.UpdateAsync(skill), Times.Once);
    }

    [Fact]
    public async Task SoftDeleteSkill_ShouldReturnNotFound_WhenSkillDoesNotExist()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        _skillRepositoryMock.Setup(repo => repo.GetByIdAsync(skillId)).ReturnsAsync((Skill)null);

        // Act
        var result = await _skillService.SoftDeleteSkill(skillId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task GetPaginatedSkillsAsync_ShouldReturnSkills_WhenSkillsExist()
    {
        // Arrange
        var skills = new List<SkillDto> { new SkillDto { Name = "Skill1" }, new SkillDto { Name = "Skill2" } };
        var totalCount = 2;
        _skillRepositoryMock.Setup(repo => repo.GetPaginatedSkillsAsync(1, 10))
            .ReturnsAsync((skills, totalCount));

        // Act
        var result = await _skillService.GetPaginatedSkillsAsync(1, 10);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.TotalCount);
        // Use Assert.Collection to verify each item in the list
        Assert.Collection(result.Value.Items,
            item => Assert.Equal("Skill1", item.Name),
            item => Assert.Equal("Skill2", item.Name));
    }

    [Fact]
    public async Task GetPaginatedSkillsAsync_ShouldReturnNotFound_WhenNoSkillsExist()
    {
        // Arrange
        _skillRepositoryMock.Setup(repo => repo.GetPaginatedSkillsAsync(1, 10))
            .ReturnsAsync((new List<SkillDto>(), 0));

        // Act
        var result = await _skillService.GetPaginatedSkillsAsync(1, 10);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task GetAllSkillsWithCoursesAsync_ShouldReturnSkills_WhenSkillsExist()
    {
        // Arrange
        var skillsWithCourses = new List<SkillWithCoursesDto> { new SkillWithCoursesDto(), new SkillWithCoursesDto() };
        _skillRepositoryMock.Setup(repo => repo.GetAllSkillsWithCoursesAsync())
            .ReturnsAsync(skillsWithCourses);

        // Act
        var result = await _skillService.GetAllSkillsWithCoursesAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(skillsWithCourses, result.Value);
    }

    [Fact]
    public async Task GetAllSkillsWithCoursesAsync_ShouldReturnNotFound_WhenNoSkillsExist()
    {
        // Arrange
        _skillRepositoryMock.Setup(repo => repo.GetAllSkillsWithCoursesAsync())
            .ReturnsAsync(new List<SkillWithCoursesDto>());

        // Act
        var result = await _skillService.GetAllSkillsWithCoursesAsync();

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task AddCreatorToSkills_ShouldReturnSuccess_WhenSkillsAreUpdated()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();
        var skillIds = new List<Guid> { id1, id2, id3 };
        _skillRepositoryMock.Setup(repo => repo.AddCreatorToSkillsAsync(creatorId, skillIds))
            .ReturnsAsync(true);

        // Act
        var result = await _skillService.AddCreatorToSkillsAsync(creatorId, skillIds);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task AddCreatorToSkills_ShouldReturnNotFound_WhenSkillsAreNotUpdated()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();
        var skillIds = new List<Guid> { id1, id2, id3 };
        _skillRepositoryMock.Setup(repo => repo.AddCreatorToSkillsAsync(creatorId, skillIds))
            .ReturnsAsync(false);

        // Act
        var result = await _skillService.AddCreatorToSkillsAsync(creatorId, skillIds);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task GetCreatorSkillsById_ShouldReturnSkills_WhenSkillsExist()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var skills = new List<SkillDto> { new SkillDto { Name = "Skill1" }, new SkillDto { Name = "Skill2" } };
        _skillRepositoryMock.Setup(repo => repo.GetCreatorSkillsByIdAsync(creatorId))
            .ReturnsAsync(skills);

        // Act
        var result = await _skillService.GetCreatorSkillsById(creatorId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(skills, result.Value);
    }
}