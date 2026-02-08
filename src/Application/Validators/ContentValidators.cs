using Application.DTOs;
using Application.DTOs.Content;

namespace Application.Validators;

using FluentValidation;
using System;
using System.Collections.Generic;

public class CourseCreateUpdateDtoValidator : AbstractValidator<CourseCreateUpdateDto>
{
    public CourseCreateUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.SkillId)
            .NotEmpty().WithMessage("SkillId is required.");

        RuleFor(x => x.CreatorId)
            .NotEmpty().WithMessage("CreatorId is required.");

        RuleFor(x => x.Videos)
            .NotEmpty().WithMessage("At least one video is required.")
            .Must(v => v != null && v.Count > 0).WithMessage("Videos collection cannot be empty.");
        
        RuleForEach(x => x.Videos).SetValidator(new VideoCreateUpdateDtoValidator());
    }
}

public class VideoCreateUpdateDtoValidator : AbstractValidator<VideoCreateUpdateDto>
{
    public VideoCreateUpdateDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

        RuleFor(x => x.ThumbnailTime)
            .Matches(@"^\d+s$").WithMessage("ThumbnailTime must be in the format 'Xs' where X is a number.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("CourseId is required.");
    }
}

public class SkillCreateUpdateDtoValidator : AbstractValidator<SkillCreateUpdateDto>
{
    public SkillCreateUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}

public class ReviewCreateUpdateDtoValidator : AbstractValidator<ReviewCreateUpdateDto>
{
    public ReviewCreateUpdateDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(1000).WithMessage("Content cannot exceed 1000 characters.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(0, 5).WithMessage("Rating must be between 0 and 5.");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("CourseId is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }
    
    
    public class PaginationQueryValidator : AbstractValidator<PaginationQuery>
    {
        public PaginationQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1).WithMessage("Page must be greater than or equal to 1.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1).WithMessage("PageSize must be greater than or equal to 1.")
                .LessThanOrEqualTo(100).WithMessage("PageSize must be less than or equal to 100.");
        }
    }
}
