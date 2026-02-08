using Application.Authorization;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Content;
using Application.Common.Interfaces.repos;
using Application.Common.Operation;
using Application.DTOs.Content;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;

namespace Web.Controllers.Content;

    [ApiController]
    [Route("api/courses")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ICourseRepository _courseRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly ICdnService _cdnService;
        
        private const string CanManageCourses = PolicyName.CanManageCourses;
        private const string Admin = RoleName.Admin;
        private const string Creator = RoleName.Creator;
        
        private const string AdminOrCreator = Admin + "," + Creator;

        public CourseController(ICourseService courseService, ICourseRepository courseRepository, IAuthorizationService authorizationService, ICdnService cdnService)
        {
            _courseService = courseService;
            _courseRepository = courseRepository;
            _authorizationService = authorizationService;
            _cdnService = cdnService;
        }
        
        #region Get Course with Videos Documentation
        /// <summary>
        /// Retrieves a course with its associated videos.
        /// </summary>
        /// <param name="courseId">The ID of the course to retrieve.</param>
        /// <returns>The course with videos if found, 404 if not found.</returns>
        [ProducesResponseType(typeof(CourseWithVideosDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        #endregion
        [Authorize]
        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetCourseWithVideosAsync(Guid courseId)
        {
            
            var result = await _courseService.GetCourseWithVideosAsync(courseId);
            return result.ToActionResult();
        }
        #region Create Course Documentation
        /// <summary>
        /// Creates a new course.
        /// </summary>
        /// <param name="courseDto">The course data to create.</param>
        /// <returns>201 Created if successful, 400 if validation fails.</returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        #endregion
        [Authorize(Roles = Creator)]
        [HttpPost]
        public async Task<IActionResult> CreateCourseAsync([FromBody] CourseCreateUpdateDto courseDto)
        {
            var videoIds = courseDto.Videos.Select(v => v.providerVideoId).ToList();
            var videosStatus = await _cdnService.CheckUploadStatusAsync(videoIds);
            
            // Collect video IDs that were not uploaded successfully
            var failedVideoIds = videosStatus
                .Where(kvp => !kvp.Value) // Check for status false
                .Select(kvp => kvp.Key) // Get the video IDs
                .ToList();

            // If there are any failed uploads, return a 400 Bad Request with the failed video IDs
            if (failedVideoIds.Any())
            {
                return BadRequest(new { message = "The following videos were not uploaded successfully.", failedVideoIds });
            }
            
            
            var result = await _courseService.CreateCourseAsync(courseDto);
            return result.ToActionResult();
        }
        
        #region Update Course Documentation
        /// <summary>
        /// Updates an existing course.
        /// </summary>
        /// <param name="courseId">The ID of the course to update.</param>
        /// <param name="courseDto">The updated course data.</param>
        /// <returns>204 No Content if successful, 404 if not found, 400 if validation fails.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        #endregion
        [Authorize(Roles = AdminOrCreator)]
        [HttpPut("{courseId}")]
        public async Task<IActionResult> UpdateCourseAsync(Guid courseId, [FromBody] CourseCreateUpdateDto courseDto)
        {
            var ownerId = _courseRepository.GetOwnerIdAsync(courseId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, ownerId, CanManageCourses);
        
            if (!authorizationResult.Succeeded)
            {
                return new ForbidResult();
            }
            var result = await _courseService.UpdateCourseAsync(courseId, courseDto);
            return result.ToActionResult();
        }

        #region Delete Course Documentation
        /// <summary>
        /// Deletes an existing course.
        /// </summary>
        /// <param name="courseId">The ID of the course to delete.</param>
        /// <returns>204 No Content if successful, 404 if not found.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        #endregion
        [Authorize(Roles = AdminOrCreator)]
        [HttpDelete("{courseId}")]
        public async Task<IActionResult> DeleteCourseAsync(Guid courseId)
        {
            var ownerId = _courseRepository.GetOwnerIdAsync(courseId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, ownerId, CanManageCourses);
        
            if (!authorizationResult.Succeeded)
            {
                return new ForbidResult();
            }
            var result = await _courseService.DeleteCourseAsync(courseId);
            return result.ToActionResult();
        }
        
        #region Make Course Paid Documentation
        /// <summary>
        /// Marks a course as paid.
        /// </summary>
        /// <param name="courseId">The ID of the course to mark as paid.</param>
        /// <returns>204 No Content if successful, 404 if not found.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        #endregion
        [Authorize(Roles = AdminOrCreator)]
        [HttpPost("{courseId}/paid")]
        public async Task<IActionResult> MakeCoursePaidAsync(Guid courseId)
        {
            var ownerId = _courseRepository.GetOwnerIdAsync(courseId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, ownerId, CanManageCourses);
        
            if (!authorizationResult.Succeeded)
            {
                return new ForbidResult();
            }
            var result = await _courseService.MakeCoursePaidAsync(courseId);
            return result.ToActionResult();
        }
        #region Make Course Free Documentation
        /// <summary>
        /// Marks a course as free.
        /// </summary>
        /// <param name="courseId">The ID of the course to mark as free.</param>
        /// <returns>204 No Content if successful, 404 if not found.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        #endregion
        [Authorize(Roles = AdminOrCreator)]
        [HttpPost("{courseId}/free")]
        public async Task<IActionResult> MakeCourseFreeAsync(Guid courseId)
        {
            var ownerId = _courseRepository.GetOwnerIdAsync(courseId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, ownerId, CanManageCourses);
        
            if (!authorizationResult.Succeeded)
            {
                return new ForbidResult();
            }
            var result = await _courseService.MakeCourseFreeAsync(courseId);
            return result.ToActionResult();
        }
        
        #region Make Course Private Documentation
        /// <summary>
        /// Makes a course private.
        /// </summary>
        /// <param name="courseId">The ID of the course to make private.</param>
        /// <returns>204 No Content if successful, 404 if not found.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        #endregion
        [Authorize(Roles = AdminOrCreator)]
        [HttpPost("{courseId}/private")]
        public async Task<IActionResult> MakeCoursePrivateAsync(Guid courseId)
        {
            var ownerId = _courseRepository.GetOwnerIdAsync(courseId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, ownerId, CanManageCourses);
        
            if (!authorizationResult.Succeeded)
            {
                return new ForbidResult();
            }
            var result = await _courseService.MakeCoursePrivateAsync(courseId);
            return result.ToActionResult();
        }
        #region Make Course Public Documentation
        /// <summary>
        /// Makes a course public.
        /// </summary>
        /// <param name="courseId">The ID of the course to make public.</param>
        /// <returns>204 No Content if successful, 404 if not found.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        #endregion
        [Authorize(Roles = AdminOrCreator)]
        [HttpPost("{courseId}/public")]
        public async Task<IActionResult> MakeCoursePublicAsync(Guid courseId)
        {
            var ownerId = _courseRepository.GetOwnerIdAsync(courseId);
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, ownerId, CanManageCourses);
        
            if (!authorizationResult.Succeeded)
            {
                return new ForbidResult();
            }
            
            var result = await _courseService.MakeCoursePublicAsync(courseId);
            return result.ToActionResult();
        }
        

    }