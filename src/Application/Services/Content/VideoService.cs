using Application.Common;
using Application.Common.Interfaces.Content;
using Application.Common.Interfaces.repos;
using Application.Common.Operation;
using Application.DTOs.Content;
using Domain.Models;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace Application.Services.Content;

public class VideoService : IVideoService
{
    private readonly IVideoRepository _videoRepository;

    public VideoService(IVideoRepository videoRepository)
    {
        _videoRepository = videoRepository;
    }

    public async Task<OperationResult<VideoDto>> GetVideoByIdAsync(Guid id)
    {
        var video = await _videoRepository.GetByIdAsync(id);
        
        if (video == null)
        {
            return OperationResult<VideoDto>.ResourceNotFound("Video", id);
        }

        var videoDto = video.Adapt<VideoDto>(); // Assuming you're using Mapster or similar for DTO mapping
        return OperationResult<VideoDto>.Success(videoDto);
    }

    public async Task<OperationResult> CreateVideoAsync(VideoCreateUpdateDto videoDto)
    {
        if (videoDto == null)
        {
            return OperationResult.ArgumentNullError("Video");
        }
        
        var providerName = videoDto.ProviderName;

        var video = new Video
        {
            // Map the properties from videoDto to the Video entity
            Id = Guid.NewGuid(),
            Title = videoDto.Title,
            Description = videoDto.Description,
            Slug = SlugHelper.GenerateSlug(videoDto.Title),
            CourseId = videoDto.CourseId,
            CreatorId = videoDto.CreatorId,
            ProviderName = videoDto.ProviderName,
            ProviderVideoId = videoDto.providerVideoId,
            IsPaid = videoDto.IsPaid,
            IsPrivate = videoDto.IsPrivate,
            
            // Add other properties as needed
        };

        await _videoRepository.AddAsync(video);
        return OperationResult.Success();
    }

    public async Task<OperationResult> UpdateVideoAsync(Guid id, VideoCreateUpdateDto videoDto)
    {
        var video = await _videoRepository.GetByIdAsync(id);
      
        
        if (video == null)
        {
            return OperationResult<VideoDto>.ResourceNotFound("Video", id);
        }

        // Update video properties with values from videoDto
        video.Title = videoDto.Title;
        video.Description = videoDto.Description;

        await _videoRepository.UpdateAsync(video);
        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteVideoAsync(Guid id)
    {
        var video = await _videoRepository.GetByIdAsync(id);
        
        if (video == null)
        {
            return OperationResult<VideoDto>.ResourceNotFound("Video", id);
        }
        

        await _videoRepository.DeleteAsync(id);
        return OperationResult.Success();
    }
    
        public async Task<OperationResult> SoftDeleteVideoAsync(Guid videoId)
    {
        var video = await _videoRepository.GetByIdAsync(videoId);
        if (video == null)
        {
            return OperationResult<Video>.ResourceNotFound("Video", videoId);
        }

        if (!video.SoftDelete())
        {
            return OperationResult.Success(SuccessTypes.NotModified);
        }

        await _videoRepository.UpdateAsync(video);
     

        return OperationResult.Success();
    }

    public async Task<OperationResult> MakeVideoPrivateAsync(Guid videoId)
    {
        var video = await _videoRepository.GetByIdAsync(videoId);
        if (video == null)
        {
            return OperationResult<Video>.ResourceNotFound("Video", videoId);
        }

        if (!video.MakePrivate())
        {
            return OperationResult.Success(SuccessTypes.NotModified);
        }

        await _videoRepository.UpdateAsync(video);
       

        return OperationResult.Success();
    }

    public async Task<OperationResult> MakeVideoPublicAsync(Guid videoId)
    {
        var video = await _videoRepository.GetByIdAsync(videoId);
        if (video == null)
        {
            return OperationResult<Video>.ResourceNotFound("Video", videoId);
        }

        if (!video.MakePublic())
        {
            return OperationResult.Success(SuccessTypes.NotModified);
        }

        await _videoRepository.UpdateAsync(video);
       

        return OperationResult.Success();
    }

    public async Task<OperationResult> MakeVideoPaidAsync(Guid videoId)
    {
        var video = await _videoRepository.GetByIdAsync(videoId);
        if (video == null)
        {
            return OperationResult<Video>.ResourceNotFound("Video", videoId);
        }

        if (!video.MakePaid())
        {
            return OperationResult.Success(SuccessTypes.NotModified);
        }

        await _videoRepository.UpdateAsync(video);
      

        return OperationResult.Success();
    }

    public async Task<OperationResult> MakeVideoFreeAsync(Guid videoId)
    {
        var video = await _videoRepository.GetByIdAsync(videoId);
        if (video == null)
        {
            return OperationResult<Video>.ResourceNotFound("Video", videoId);
        }

        if (!video.MakeFree())
        {
            return OperationResult.Success(SuccessTypes.NotModified);
        }

        await _videoRepository.UpdateAsync(video);
      

        return OperationResult.Success();
    }

    public async Task<OperationResult<(List<VideoDto> Items, int TotalCount)>> GetPaginatedByCourseIdAsync
                                                           ( Guid courseId, int? page = null, int? pageSize = null)
    {
        var result = await _videoRepository.GetPaginatedByCourseIdAsync(courseId, page, pageSize);
        if (result.Videos is null || result.TotalCount == 0)
        {
            return OperationResult<(List<VideoDto> Items, int TotalCount)>.ResourceNotFound("Videos for Course", courseId);
            
        }
        return OperationResult<(List<VideoDto> Items, int TotalCount)>.Success(result);
    }
}

