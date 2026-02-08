using Application.DTOs.Identity;
using FluentValidation;
namespace Application.Validators
{
	public class UserValidators
	{
		public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
		{
			public CreateUserDtoValidator()
			{
				RuleFor(x => x.UserName)
					.NotEmpty().WithMessage("Username is required.")
					.MaximumLength(256).WithMessage("Username must not exceed 256 characters.");

				RuleFor(x => x.Email)
					.NotEmpty().WithMessage("Email is required.")
					.EmailAddress().WithMessage("Email format is invalid.")
					.MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

				RuleFor(x => x.FirstName)
					.NotEmpty().WithMessage("First name is required.")
					.MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

				RuleFor(x => x.LastName)
					.MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");
				
				RuleFor(x => x.DateOfBirth)
					.Must(date => date.HasValue)
					.WithMessage("Date of Birth is required.")
					.Must(date => date.Value < DateOnly.FromDateTime(DateTime.Now))
					.WithMessage("Date of Birth must be in the past.")
					.InclusiveBetween(DateOnly.FromDateTime(DateTime.Now.AddYears(-120)), DateOnly.FromDateTime(DateTime.Now))
					.WithMessage("Date of Birth must be between 120 years ago and now.");

				RuleFor(x => x.Password)
					.NotEmpty().WithMessage("Password is required.")
					.MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
					.Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
					.Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
					.Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
					.Matches(@"[\W]").WithMessage("Password must contain at least one special character.");
			}
		}

		public class UpdateUserProfileDtoValidator : AbstractValidator<UpdateUserProfileDto>
		{
			public UpdateUserProfileDtoValidator()
			{
				RuleFor(x => x.UserId)
					.NotEmpty().WithMessage("User ID is required.");

				RuleFor(x => x.FirstName)
					.MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

				RuleFor(x => x.LastName)
					.MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

				RuleFor(x => x.UserName)
					.MaximumLength(256).WithMessage("Username must not exceed 256 characters.");

				RuleFor(x => x.Email)
					.EmailAddress().WithMessage("Email format is invalid.")
					.MaximumLength(256).WithMessage("Email must not exceed 256 characters.");
			}
		}

		public class UpdatePasswordDtoValidator : AbstractValidator<UpdatePasswordDto>
		{
			public UpdatePasswordDtoValidator()
			{
				RuleFor(x => x.UserId)
					.NotEmpty().WithMessage("User ID is required.");

				RuleFor(x => x.CurrentPassword)
					.NotEmpty().WithMessage("Current password is required.");

				RuleFor(x => x.NewPassword)
					.NotEmpty().WithMessage("New password is required.")
					.MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
					.Matches(@"[A-Z]").WithMessage("New password must contain at least one uppercase letter.")
					.Matches(@"[a-z]").WithMessage("New password must contain at least one lowercase letter.")
					.Matches(@"[0-9]").WithMessage("New password must contain at least one number.")
					.Matches(@"[\W]").WithMessage("New password must contain at least one special character.");
			}
		}
		
		public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
		{
			public ResetPasswordDtoValidator()
			{
				RuleFor(x => x.Email)
					.NotEmpty().WithMessage("Email is required.")
					.EmailAddress().WithMessage("Invalid email format.");

				RuleFor(x => x.NewPassword)
					.NotEmpty().WithMessage("Password is required.")
					.MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
					.Matches(@"[A-Z]").WithMessage("New password must contain at least one uppercase letter.")
					.Matches(@"[a-z]").WithMessage("New password must contain at least one lowercase letter.")
					.Matches(@"[0-9]").WithMessage("New password must contain at least one number.")
					.Matches(@"[\W]").WithMessage("New password must contain at least one special character.");

				RuleFor(x => x.Token)
					.NotEmpty().WithMessage("Token is required.");
			}
		}

		
	}
}
