using Application.Authorization;
using Application.Common.Interfaces.Content;
using Application.Common.Interfaces.repos;
using Application.DTOs.Content;
using Microsoft.AspNetCore.Authorization;
using Web.Extensions;

namespace Web.Controllers.Content;

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/videos")]
public class VideoController : ControllerBase
{
    private readonly IVideoService _videoService;
    private readonly IVideoRepository _videoRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly ICdnService _cdnService;
    
    private const string CanManageVideos = PolicyName.CanManageCourses;
    private const string Creator = RoleName.Creator;
    private const string Admin = RoleName.Admin;
    private const string AdminOrCreator = Admin + "," + Creator;

    public VideoController(IVideoService videoService, IAuthorizationService authorizationService, IVideoRepository videoRepository, ICdnService cdnService)
    {
        _videoService = videoService;
        _authorizationService = authorizationService;
        _videoRepository = videoRepository;
        _cdnService = cdnService;
    }
    
    [Authorize(Roles = AdminOrCreator)]
    [HttpGet("upload-token")]
    public async Task<IActionResult> GetUploadToken()
    {
        var uploadToken = await _cdnService.GenerateUploadTokenAsync();
        return Ok(new { UploadToken = uploadToken });
    }

    #region GetVideoById

    /// <summary>
    /// Retrieves a video by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the video.</param>
    /// <returns>A video with the specified identifier.</returns>
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(VideoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVideoById(Guid id)
    {
        var result = await _videoService.GetVideoByIdAsync(id);
        return result.ToActionResult();
    }

    #endregion

    #region CreateVideo

    /// <summary>
    /// Creates a new video.
    /// </summary>
    /// <param name="videoDto">The video data transfer object containing video details.</param>
    /// <returns>Response indicating the result of the video creation process.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateVideo([FromBody] VideoCreateUpdateDto videoDto)
    {
        var result = await _videoService.CreateVideoAsync(videoDto);
        return result.ToActionResult();
    }

    #endregion

    #region UpdateVideo

    /// <summary>
    /// Updates an existing video by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the video to update.</param>
    /// <param name="videoDto">The video data transfer object containing updated video details.</param>
    /// <returns>Response indicating the result of the update process.</returns>
    [Authorize(Roles = AdminOrCreator)]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateVideo(Guid id, [FromBody] VideoCreateUpdateDto videoDto)
    {
        var result = await _videoService.UpdateVideoAsync(id, videoDto);
        return result.ToActionResult();
    }

    #endregion

    #region DeleteVideo

    /// <summary>
    /// Permanently deletes a video by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the video to delete.</param>
    /// <returns>Response indicating the result of the deletion process.</returns>
    [Authorize(Roles = Admin)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVideo(Guid id)
    {
        var result = await _videoService.DeleteVideoAsync(id);
        return result.ToActionResult();
    }

    #endregion

    #region SoftDeleteVideo

    /// <summary>
    /// Soft deletes a video by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the video to soft delete.</param>
    /// <returns>Response indicating the result of the soft deletion process.</returns>
    [Authorize(Roles = AdminOrCreator)]
    [HttpPatch("{id}/soft-delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SoftDeleteVideo(Guid id)
    {
        var ownerId = _videoRepository.GetOwnerIdAsync(id);
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, ownerId, CanManageVideos);
        
        if (!authorizationResult.Succeeded)
        {
            return new ForbidResult();
        }
        var result = await _videoService.SoftDeleteVideoAsync(id);
        return result.ToActionResult();
    }

    #endregion

    #region MakeVideoPrivate

    /// <summary>
    /// Marks a video as private.
    /// </summary>
    /// <param name="id">The unique identifier of the video to mark as private.</param>
    /// <returns>Response indicating the result of the update process.</returns>
    [Authorize(Roles = AdminOrCreator)]
    [HttpPatch("{id}/make-private")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MakeVideoPrivate(Guid id)
    {
        var ownerId = _videoRepository.GetOwnerIdAsync(id);
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, ownerId, CanManageVideos);
        
        if (!authorizationResult.Succeeded)
        {
            return new ForbidResult();
        }
        var result = await _videoService.MakeVideoPrivateAsync(id);
        return result.ToActionResult();
    }

    #endregion

    #region MakeVideoPublic

    /// <summary>
    /// Marks a video as public.
    /// </summary>
    /// <param name="id">The unique identifier of the video to mark as public.</param>
    /// <returns>Response indicating the result of the update process.</returns>
    [Authorize(Roles = AdminOrCreator)]
    [HttpPatch("{id}/make-public")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MakeVideoPublic(Guid id)
    {
        var ownerId = _videoRepository.GetOwnerIdAsync(id);
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, ownerId, CanManageVideos);
        
        if (!authorizationResult.Succeeded)
        {
            return new ForbidResult();
        }
        var result = await _videoService.MakeVideoPublicAsync(id);
        return result.ToActionResult();
    }

    #endregion

    #region MakeVideoPaid

    /// <summary>
    /// Marks a video as paid.
    /// </summary>
    /// <param name="id">The unique identifier of the video to mark as paid.</param>
    /// <returns>Response indicating the result of the update process.</returns>
    [HttpPatch("{id}/make-paid")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MakeVideoPaid(Guid id)
    {
        var ownerId = _videoRepository.GetOwnerIdAsync(id);
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, ownerId, CanManageVideos);
        
        if (!authorizationResult.Succeeded)
        {
            return new ForbidResult();
        }
        var result = await _videoService.MakeVideoPaidAsync(id);
        return result.ToActionResult();
    }

    #endregion

    #region MakeVideoFree

    /// <summary>
    /// Marks a video as free.
    /// </summary>
    /// <param name="id">The unique identifier of the video to mark as free.</param>
    /// <returns>Response indicating the result of the update process.</returns>
    [Authorize(Roles = AdminOrCreator)]
    [HttpPatch("{id}/make-free")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MakeVideoFree(Guid id)
    {
        var ownerId = _videoRepository.GetOwnerIdAsync(id);
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, ownerId, CanManageVideos);
        
        if (!authorizationResult.Succeeded)
        {
            return new ForbidResult();
        }
        var result = await _videoService.MakeVideoFreeAsync(id);
        return result.ToActionResult();
    }

    #endregion


}
