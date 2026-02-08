using Application.Common;
using Application.Common.Interfaces.Content;
using Application.Common.Interfaces.repos;
using Application.Common.Operation;
using Application.DTOs.Content;
using Domain.Models;
using Mapster;


namespace Application.Services.Content;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IVideoRepository _videoRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(ICourseRepository courseRepository, IVideoRepository videoRepository,
                        IReviewRepository reviewRepository, IUnitOfWork unitOfWork)
    {
        _courseRepository = courseRepository;
        _videoRepository = videoRepository;
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<OperationResult<CourseWithVideosDto>> GetCourseWithVideosAsync(Guid courseId)
    {
        var result = await _courseRepository.GetCourseWithVideosAsync(courseId);
        if (result == null)
        {
            return OperationResult<CourseWithVideosDto>.ResourceNotFound("Course", courseId);
        }

        return OperationResult<CourseWithVideosDto>.Success(result);
    }

    public async Task<OperationResult> CreateCourseAsync(CourseCreateUpdateDto courseDto)
    {
        if (courseDto == null)
        {
            return OperationResult<Course>.ArgumentNullError("Course");
        }

        var slug = SlugHelper.GenerateSlug(courseDto.Title);

        // Create a new Course object and map properties from the DTO
        var course = new Course
        {
            Title = courseDto.Title,
            Slug = slug,
            Description = courseDto.Description,
            CreatorId = courseDto.CreatorId, // Using CreatorId from the DTO
            SkillId = courseDto.SkillId,
            IsPaid = courseDto.IsPaid,
            Price = courseDto.Price,
            IsPrivate = courseDto.IsPrivate
        };

        // Add the course to the repository (but don't save yet)
        await _courseRepository.AddWithNoSaveAsync(course);

        // Check if there are videos to add
        if (courseDto.Videos != null && courseDto.Videos.Any())
        {
            // Map each VideoCreateUpdateDto to Video using Mapster
            var videos = courseDto.Videos.Select(videoDto => 
            {
                var video = videoDto.Adapt<Video>(); // Map properties from DTO to Video
                video.CourseId = course.Id;
                video.CreatorId = course.CreatorId;
                video.Slug = SlugHelper.GenerateSlug(videoDto.Title);
                return video;
            }).ToList();

            // Add videos to the repository
            foreach (var video in videos)
            {
                await _videoRepository.AddWithNoSaveAsync(video);
            }
        }

        // Save all changes through the Unit of Work
        await _unitOfWork.CommitAsync();

        return OperationResult.Success();
    }


    public async Task<OperationResult> UpdateCourseAsync(Guid courseId, CourseCreateUpdateDto courseDto)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            return OperationResult<Course>.ResourceNotFound("Course", courseId);
        }

        if (course.Title != courseDto.Title)
        {
            course.Slug = SlugHelper.GenerateSlug(courseDto.Title);
        }

        course.Title = courseDto.Title;
        course.Description = courseDto.Description;
        course.IsPaid = courseDto.IsPaid;
        course.Price = courseDto.Price;

      await  _courseRepository.UpdateAsync(course);
       

        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteCourseAsync(Guid courseId)
    {

        var course = await _courseRepository.GetByIdAsync(courseId);
        
        if (course == null)
        {
                return OperationResult.ResourceNotFound("Course", courseId);
        }

        var deleteResult = course.SoftDelete();
        if (!deleteResult) return OperationResult.Success(SuccessTypes.NotModified);
        
        var videos = await _videoRepository.GetVideosByCourseIdAsync(courseId);
        foreach (var video in videos)
        {
            video.SoftDelete();
        }
        
        _videoRepository.UpdateBatchWithNoSave(videos);
        _courseRepository.UpdateWithNoSave(course);
        await _unitOfWork.CommitAsync();


        return OperationResult.Success();
        
    }
    public async Task<OperationResult> MakeCoursePaidAsync(Guid courseId)
    {

        var course = await _courseRepository.GetByIdAsync(courseId);
        
        if (course == null)
        {
            return OperationResult.ResourceNotFound("Course", courseId);
        }

        var paidResult = course.MakePaid();
        if (!paidResult) return OperationResult.Success(SuccessTypes.NotModified);
        
        var videos = await _videoRepository.GetVideosByCourseIdAsync(courseId);
        foreach (var video in videos)
        {
            video.MakePaid();
        }
        
        _videoRepository.UpdateBatchWithNoSave(videos);
        _courseRepository.UpdateWithNoSave(course);
        await _unitOfWork.CommitAsync();


        return OperationResult.Success();
        
    }
    public async Task<OperationResult> MakeCourseFreeAsync(Guid courseId)
    {

        var course = await _courseRepository.GetByIdAsync(courseId);
        
        if (course == null)
        {
            return OperationResult.ResourceNotFound("Course", courseId);
        }

        var paidResult = course.MakeFree();
        if (!paidResult) return OperationResult.Success(SuccessTypes.NotModified);
        
        var videos = await _videoRepository.GetVideosByCourseIdAsync(courseId);
        foreach (var video in videos)
        {
            video.MakeFree();
        }
        
        _videoRepository.UpdateBatchWithNoSave(videos);
        _courseRepository.UpdateWithNoSave(course);
        await _unitOfWork.CommitAsync();


        return OperationResult.Success();
        
    }
    
    public async Task<OperationResult> MakeCoursePrivateAsync(Guid courseId)
    {

        var course = await _courseRepository.GetByIdAsync(courseId);
        
        if (course == null)
        {
            return OperationResult.ResourceNotFound("Course", courseId);
        }

        var privateResult = course.MakePrivate();
        if (!privateResult) return OperationResult.Success(SuccessTypes.NotModified);
        
        var videos = await _videoRepository.GetVideosByCourseIdAsync(courseId);
        foreach (var video in videos)
        {
            video.MakePrivate();
        }
        
        _videoRepository.UpdateBatchWithNoSave(videos);
        _courseRepository.UpdateWithNoSave(course);
        await _unitOfWork.CommitAsync();


        return OperationResult.Success();
        
    }
    
    public async Task<OperationResult> MakeCoursePublicAsync(Guid courseId)
    {

        var course = await _courseRepository.GetByIdAsync(courseId);
        
        if (course == null)
        {
            return OperationResult.ResourceNotFound("Course", courseId);
        }

        var privateResult = course.MakePublic();
        if (!privateResult) return OperationResult.Success(SuccessTypes.NotModified);
        
        var videos = await _videoRepository.GetVideosByCourseIdAsync(courseId);
        foreach (var video in videos)
        {
            video.MakePublic();
        }
        
        _videoRepository.UpdateBatchWithNoSave(videos);
        _courseRepository.UpdateWithNoSave(course);
        await _unitOfWork.CommitAsync();


        return OperationResult.Success();
        
    }


    public async Task<OperationResult<(List<CourseDto> Courses, int TotalCount)>> GetCoursesBySkillIdAsync(Guid skillId, int? page = null, int? pageSize = null)
    {
        var result = await _courseRepository.GetCoursesBySkillIdAsync(skillId, page, pageSize);
        if (result.Courses == null || result.TotalCount == 0)
        {
            return OperationResult<(List<CourseDto> Courses, int TotalCount)>.ResourceNotFound("Courses for Skill", skillId);
        }

        return OperationResult<(List<CourseDto>, int TotalCount)>.Success(result);
        
    }



}
