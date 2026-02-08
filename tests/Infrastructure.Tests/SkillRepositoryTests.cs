using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Infrastructure.Tests;

public class SkillRepositoryTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly ApplicationDbContext _context;
    private readonly SkillRepository _repository;
    
    private Guid _userId = Guid.NewGuid();

    public SkillRepositoryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique name for each test
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new SkillRepository(_context);
        
        _context.Users.Add(new User
        {
            Id = _userId,
            FirstName = "John",
            IsActive = true

        });
    }

    [Fact]
    public async Task GetAllAsIEnumerableAsync_ShouldReturnAllSkills()
    {
        // Arrange
        var skills = new List<Skill>
        {
            new Skill { Id = Guid.NewGuid(), Name = "Skill 1" , Description = "Description1", Slug = "name-1"},
            new Skill { Id = Guid.NewGuid(), Name = "Skill 2", Description = "Description2", Slug = "name-2"},
        };
        _context.Skills.AddRange(skills);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsIEnumerableAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnSkill_WhenExists()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var skill = new Skill { Id = skillId, Name = "Skill 1", Description = "Description 1" , Slug = "Slug 1" };
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(skillId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(skillId, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddSkill()
    {
        // Arrange
        var skill = new Skill { Id = Guid.NewGuid(), Name = "Skill 1", Description = "Description 1" , Slug = "Slug 1" };

        // Act
        await _repository.AddAsync(skill);

        // Assert
        var addedSkill = await _context.Skills.FindAsync(skill.Id);
        Assert.NotNull(addedSkill);
        Assert.Equal(skill.Name, addedSkill.Name);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSkill()
    {
        // Arrange
        var skill = new Skill { Id = Guid.NewGuid(), Name = "Skill 1", Description = "Description 1" , Slug = "Slug 1" };
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        skill.Name = "Updated Skill";

        // Act
        await _repository.UpdateAsync(skill);

        // Assert
        var updatedSkill = await _context.Skills.FindAsync(skill.Id);
        Assert.NotNull(updatedSkill);
        Assert.Equal("Updated Skill", updatedSkill.Name);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveSkill_WhenExists()
    {
        // Arrange
        var skillId = Guid.NewGuid();
        var skill = new Skill { Id = Guid.NewGuid(), Name = "Skill 1", Description = "Description 1" , Slug = "Slug 1" };
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(skillId);

        // Assert
        var deletedSkill = await _context.Skills.FindAsync(skillId);
        Assert.Null(deletedSkill);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotRemoveSkill_WhenNotExists()
    {
        // Act
        await _repository.DeleteAsync(Guid.NewGuid());

        // Assert
        var allSkills = await _context.Skills.ToListAsync();
        Assert.Empty(allSkills);
    }

    // Additional tests for methods like GetSkillWithCoursesByIdAsync, GetAllSkillsWithPaginatedCoursesAsync, etc.
    [Fact]
    public async Task GetSkillWithCoursesByIdAsync_ShouldReturnSkillWithCourses_WhenExists()
    {
        var skillId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var skill = new Skill
        {
            Id = skillId,
            Name = "Skill 1",
            Description = "Description 1",
            Slug = "skill-1",
            
        };
        var course = new Course {
            Id = courseId, 
            Title = "Course 1",
            Description = "Course 1 Description",
            Slug ="course1",
            SkillId = skillId ,
            CreatorId = _userId,
            IsPaid = false,
            IsPrivate = false};
          skill.Courses.Add(course);
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();

        var result = await _repository.GetSkillWithCoursesByIdAsync(skillId);

        Assert.NotNull(result);
        //Assert.Equal(skillId, result.Skill.Id);
        Assert.NotNull(result.CourseDtos);
        Assert.Single(result.CourseDtos);
        Assert.Equal("Course 1", result.CourseDtos.First().Title);
    }

    [Fact]
    public async Task GetAllSkillsWithPaginatedCoursesAsync_ShouldReturnPaginatedSkillsWithCourses()
    {
        var skill1Id = Guid.NewGuid();
        var skill2Id = Guid.NewGuid();
        var skill1 = new Skill { Id = skill1Id, Name = "Skill 1", Description = "Description 1", Slug = "skill-1" };
        var skill2 = new Skill { Id = skill2Id, Name = "Skill 2", Description = "Description 2", Slug = "skill-2" };
        var course1 = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Test Course1",
            Description = "Description1",
            Slug = "test-course",
            CreatorId = Guid.NewGuid(),
            SkillId = skill1Id ,
            IsPaid = true,
            IsPrivate = false
        };
        var course2 = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Test Course2",
            Description = "Description2",
            Slug = "test-course",
            CreatorId = Guid.NewGuid(),
            SkillId = skill1Id,
            IsPaid = true,
            IsPrivate = false

        };
        var course3 = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Test Course2",
            Description = "Description2",
            Slug = "test-course",
            CreatorId = Guid.NewGuid(),
            SkillId = skill2Id,
            IsPaid = true,
            IsPrivate = false

        };
        var course4 = new Course
        {
            Id = Guid.NewGuid(),
            Title = "Test Course2",
            Description = "Description2",
            Slug = "test-course",
            CreatorId = Guid.NewGuid(),
            SkillId = skill2Id,
            IsPaid = true,
            IsPrivate = false

        };
        

        _context.Skills.AddRange(skill1, skill2);
        _context.Courses.AddRange(course1, course2, course3, course4);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllSkillsWithPaginatedCoursesAsync(page: 1, pageSize: 1);

        Assert.NotNull(result);
        Assert.Equal(2, result.First().TotalCourseCount);
        Assert.Equal(2, result.Last().TotalCourseCount);
        Assert.NotNull(result.First().CourseDtos);
        Assert.NotNull(result.Last().CourseDtos);
        Assert.Equal("Skill 1", result.First().Skill.Name);
        Assert.Equal("Skill 2", result.Last().Skill.Name);
        _testOutputHelper.WriteLine(result.First().CourseDtos.First().Title);
        _testOutputHelper.WriteLine(result.Last().CourseDtos.First().Title);
    }

    [Fact]
    public async Task GetPaginatedSkillsAsync_ShouldReturnPaginatedSkills()
    {
        for (int i = 0; i < 5; i++)
        {
            var skill = new Skill { Id = Guid.NewGuid(), Name = $"Skill {i + 1}", Description = $"Description {i + 1}", Slug = $"skill-{i + 1}" };
            _context.Skills.Add(skill);
        }
        await _context.SaveChangesAsync();

        var (skills, totalCount) = await _repository.GetPaginatedSkillsAsync(page: 1, pageSize: 3);

        Assert.Equal(3, skills.Count);
        Assert.Equal(5, totalCount);
        Assert.Equal("Skill 1", skills[0].Name);
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}
