using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagement.API.Controllers;

[Authorize]
public class DevicesController(IDeviceService deviceService, IAIDescriptionService aiDescriptionService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await deviceService.GetAllAsync());

    [HttpGet("my-view")]
    public async Task<IActionResult> GetForUser()
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();
        return Ok(await deviceService.GetForUserAsync(userId.Value));
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { message = "Search query cannot be empty." });

        int? scopedUserId = IsAdmin() ? null : GetCurrentUserId();
        return Ok(await deviceService.SearchAsync(q, scopedUserId));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var device = await deviceService.GetByIdAsync(id);
        return device is null ? NotFound() : Ok(device);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] DeviceRequestDto dto)
    {
        var created = await deviceService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] DeviceRequestDto dto)
    {
        var updated = await deviceService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await deviceService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("generate-description")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GenerateDescription([FromBody] DeviceRequestDto dto)
    {
        try
        {
            var description = await aiDescriptionService.GenerateDescriptionAsync(dto);
            return Ok(new { description });
        }
        catch (InvalidOperationException ex)
        {
            return ServiceUnavailable(new { message = ex.Message });
        }
    }

    private ObjectResult ServiceUnavailable(object value) =>
        StatusCode(StatusCodes.Status503ServiceUnavailable, value);
}
