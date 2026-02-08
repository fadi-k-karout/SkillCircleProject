using Application.Authorization;
using Application.Common.Interfaces.Content;
using Application.DTOs;
using Application.DTOs.Content;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Swashbuckle.AspNetCore.Annotations;
using Web.Extensions;

namespace Web.Controllers.Content;


[Authorize(Roles = RoleName.Admin)]
[ApiController]
[Route("api/skills")]
public class SkillController : ControllerBase
{
    private readonly ISkillService _skillService;
    
    private const string AdminOrCreator  = RoleName.Admin + "," + RoleName.Creator;

    public SkillController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    [HttpPost("create")]
    #region Swagger Documentation and Response Types
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Creates a new skill", Description = "Creates a new skill with the specified details.")]
    #endregion
    public async Task<IActionResult> CreateSkillAsync([FromBody] SkillDto skillDto)
    {
        var result = await _skillService.CreateSkill(skillDto);
        return result.ToActionResult();
    }
    
    [Authorize(Roles = RoleName.Admin)]
    [HttpDelete("{skillId}")]
    #region Swagger Documentation and Response Types
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Deletes a skill", Description = "Deletes the skill with the specified ID.")]
    #endregion
    public async Task<IActionResult> DeleteSkillAsync(Guid skillId)
    {
        var result = await _skillService.SoftDeleteSkill(skillId);
        return result.ToActionResult();
    }
    [AllowAnonymous]
    [HttpGet]
    #region Swagger Documentation and Response Types
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Gets all skills", Description = "Retrieves a list of all skills.")]
    #endregion
    public async Task<IActionResult> GetSkillsAsync([FromQuery] PaginationQuery pagination )
    {
        var result = await _skillService.GetPaginatedSkillsAsync(pagination.Page.Value, pagination.PageSize.Value);
        return result.ToActionResult();
    }
    
    [AllowAnonymous]
    [HttpGet("{skillId}")]
    #region Swagger Documentation and Response Types
    [ProducesResponseType(typeof(SkillWithCoursesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Gets skill by ID", Description = "Retrieves the details of the skill with the specified ID.")]
    #endregion
    public async Task<IActionResult> GetSkillByIdAsync(Guid skillId)
    {
        var result = await _skillService.GetSkillWithCoursesById(skillId);
        return result.ToActionResult();
    }
    [Authorize(Roles = RoleName.Admin)]
    [HttpPut("{skillId}")]
    #region Swagger Documentation and Response Types
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Updates a skill", Description = "Updates the details of the skill with the specified ID.")]
    #endregion
    public async Task<IActionResult> UpdateSkillAsync(Guid skillId, [FromBody] SkillDto skillDto)
    {
        var result = await _skillService.UpdateSkill(skillId, skillDto);
        return result.ToActionResult();
    }
    
    [Authorize(Roles = AdminOrCreator)]
    [HttpPost("{creatorId}/add-to-skills")]
    #region Swagger Documentation and Response Types
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Adds a creator to skills", Description = "Adds a creator to a list of skills.")]
    #endregion
    public async Task<IActionResult> AddCreatorToSkillsAsync(Guid creatorId, [FromBody] List<Guid> skillIds)
    {
        var result = await _skillService.AddCreatorToSkillsAsync(creatorId, skillIds);
        return result.ToActionResult();
    }
    
    [AllowAnonymous]
    [HttpGet("paginated-with-courses")]
    #region Swagger Documentation and Response Types
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(Summary = "Gets all skills with paginated courses", Description = "Retrieves a paginated list of skills with their associated courses.")]
    #endregion
    public async Task<IActionResult> GetAllSkillsWithPaginatedCoursesAsync([FromQuery] PaginationQuery pagination)
    {
        var result = await _skillService.GetAllSkillsWithPaginatedCoursesAsync(pagination.Page.Value, pagination.PageSize.Value);
        return result.ToActionResult();
    }
}
