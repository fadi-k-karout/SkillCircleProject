using Application.DTOs.Content;
using Application.DTOs.Identity;
using Domain.Models;
using Mapster;

namespace Application.Common.Mappings
{
	

	public class UserMappingConfig
	{
public static void ConfigureMappings()
{
	// Mapping from CreateUserDto to User
	TypeAdapterConfig<CreateUserDto, User>
		.NewConfig()
		.Map(dest => dest.UserName, src => src.UserName)
		.Map(dest => dest.Email, src => src.Email)
		.Map(dest => dest.FirstName, src => src.FirstName)
		.Map(dest => dest.LastName, src => src.LastName)
		.Map(dest => dest.DateOfBirth, src => src.DateOfBirth);

	// Mapping from UpdateUserProfileDto to User with non-null properties only
	TypeAdapterConfig<UpdateUserProfileDto, User>
		.NewConfig()
		.IgnoreNullValues(true);

	// Mapping for User to UserPublicInfoDto
	TypeAdapterConfig<User, UserPublicInfoDto>.NewConfig()
		.Map(dest => dest.FirstName, src => src.FirstName)
		.Map(dest => dest.LastName, src => src.LastName)
		.Map(dest => dest.PhotoUrl, src => src.PhotoUrl)
		.Map(dest => dest.CreatedAt, src => src.CreatedAt);

	// Mapping for Skill to SkillDto
	TypeAdapterConfig<Skill, SkillDto>.NewConfig()
		.Map(dest => dest.Id, src => src.Id)
		.Map(dest => dest.Name, src => src.Name)
		.Map(dest => dest.Slug, src => src.Slug)
		.Map(dest => dest.Description, src => src.Description);

	// Mapping for Skill to SkillWithCoursesDto with fallback for courses list
	TypeAdapterConfig<Skill, SkillWithCoursesDto>
		.NewConfig()
		.Map(dest => dest.Skill, src => src.Adapt<SkillDto>())
		.Map(dest => dest.CourseDtos, src => src.Courses != null
			? src.Courses.Select(c => c.Adapt<CourseDto>()).ToList()
			: new List<CourseDto>());

	// Configure mapping for Review to ReviewDto
	TypeAdapterConfig<Review, ReviewDto>.NewConfig()
		.Map(dest => dest.CreatedAt, src => src.Status.CreatedAt)
		.Map(dest => dest.UpdatedAt, src => src.Status.UpdatedAt)
		.Map(dest => dest.CourseId, src => src.CourseId)
		.Map(dest => dest.Content, src => src.Content)
		.Map(dest => dest.Rating, src => src.Rating)
		.Map(dest => dest.User, src => src.User != null 
			? new UserPublicInfoDto
			{
				FirstName = src.User.FirstName,
				LastName = src.User.LastName,
				PhotoUrl = src.User.PhotoUrl,
				CreatedAt = src.User.CreatedAt
			} : null);

	// Configure mapping for Course to CourseDto with fallback for reviews list
	TypeAdapterConfig<Course, CourseDto>.NewConfig()
		.Map(dest => dest.Id, src => src.Id)
		.Map(dest => dest.Title, src => src.Title)
		.Map(dest => dest.Description, src => src.Description)
		.Map(dest => dest.Slug, src => src.Slug)
		.Map(dest => dest.IsPaid, src => src.IsPaid)
		.Map(dest => dest.NumberOfReviews, src => src.Reviews != null ? src.Reviews.Count : 0)
		.Map(dest => dest.AverageRating, src => src.Reviews != null && src.Reviews.Count > 0 
			? src.Reviews.Average(r => r.Rating) : 0)
		.Map(dest => dest.Creator, src => src.Creator != null 
			? new UserPublicInfoDto
			{
				FirstName = src.Creator.FirstName,
				LastName = src.Creator.LastName,
				PhotoUrl = src.Creator.PhotoUrl,
				CreatedAt = src.Creator.CreatedAt
			} : null);
}
	}


}
