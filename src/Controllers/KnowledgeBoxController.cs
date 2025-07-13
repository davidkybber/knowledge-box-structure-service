using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using KnowledgeBox.Structure.Models;
using KnowledgeBox.Structure.Services;

namespace KnowledgeBox.Structure.Controllers;

[ApiController]
[Route("knowledgeboxes")]
[Authorize]
public class KnowledgeBoxController : ControllerBase
{
    private readonly IKnowledgeBoxService _knowledgeBoxService;

    public KnowledgeBoxController(IKnowledgeBoxService knowledgeBoxService)
    {
        _knowledgeBoxService = knowledgeBoxService;
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
               User.FindFirst("sub")?.Value ?? 
               User.FindFirst("user_id")?.Value ?? 
               throw new UnauthorizedAccessException("User ID not found in token");
    }

    /// <summary>
    /// Get all knowledge boxes for the authenticated user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<KnowledgeBoxListResponse>> GetAllKnowledgeBoxes()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _knowledgeBoxService.GetAllKnowledgeBoxesAsync(userId);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ErrorResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Message = "Internal server error", Error = ex.Message });
        }
    }

    /// <summary>
    /// Get a specific knowledge box by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<KnowledgeBoxResponse>> GetKnowledgeBoxById(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _knowledgeBoxService.GetKnowledgeBoxByIdAsync(id, userId);
            
            if (result.Success)
                return Ok(result);
            
            return NotFound(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ErrorResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Message = "Internal server error", Error = ex.Message });
        }
    }

    /// <summary>
    /// Create a new knowledge box
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<KnowledgeBoxResponse>> CreateKnowledgeBox([FromBody] CreateKnowledgeBoxRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse 
                { 
                    Message = "Invalid request data", 
                    Error = "VALIDATION_ERROR" 
                });
            }

            var userId = GetCurrentUserId();
            var result = await _knowledgeBoxService.CreateKnowledgeBoxAsync(request, userId);
            
            if (result.Success)
                return CreatedAtAction(nameof(GetKnowledgeBoxById), new { id = result.KnowledgeBox!.Id }, result);
            
            return BadRequest(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ErrorResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Message = "Internal server error", Error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing knowledge box
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<KnowledgeBoxResponse>> UpdateKnowledgeBox(string id, [FromBody] UpdateKnowledgeBoxRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse 
                { 
                    Message = "Invalid request data", 
                    Error = "VALIDATION_ERROR" 
                });
            }

            // Ensure the ID in the URL matches the ID in the request body
            request.Id = id;

            var userId = GetCurrentUserId();
            var result = await _knowledgeBoxService.UpdateKnowledgeBoxAsync(request, userId);
            
            if (result.Success)
                return Ok(result);
            
            return NotFound(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ErrorResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Message = "Internal server error", Error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a knowledge box
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<DeleteKnowledgeBoxResponse>> DeleteKnowledgeBox(string id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _knowledgeBoxService.DeleteKnowledgeBoxAsync(id, userId);
            
            if (result.Success)
                return Ok(result);
            
            return NotFound(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ErrorResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Message = "Internal server error", Error = ex.Message });
        }
    }

    /// <summary>
    /// Search knowledge boxes by query and/or tags
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<KnowledgeBoxListResponse>> SearchKnowledgeBoxes([FromQuery] string? query, [FromQuery] string? tags)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _knowledgeBoxService.SearchKnowledgeBoxesAsync(query, tags, userId);
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ErrorResponse { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Message = "Internal server error", Error = ex.Message });
        }
    }

    /// <summary>
    /// Get all public knowledge boxes
    /// </summary>
    [HttpGet("public")]
    public async Task<ActionResult<KnowledgeBoxListResponse>> GetPublicKnowledgeBoxes()
    {
        try
        {
            var result = await _knowledgeBoxService.GetPublicKnowledgeBoxesAsync();
            
            if (result.Success)
                return Ok(result);
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ErrorResponse { Message = "Internal server error", Error = ex.Message });
        }
    }
}