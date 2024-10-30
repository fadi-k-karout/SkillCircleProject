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
				.IgnoreNullValues(true); // Ignore null values during mapping
		}
	}


}
