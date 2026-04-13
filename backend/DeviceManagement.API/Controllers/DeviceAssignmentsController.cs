using DeviceManagement.Application.DTOs;
using DeviceManagement.Application.DTOs.Auth;
using DeviceManagement.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagement.API.Controllers;

public class DeviceAssignmentsController(IDeviceAssignmentService assignmentService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await assignmentService.GetAllAsync());

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var assignment = await assignmentService.GetByIdAsync(id);
        return assignment is null ? NotFound() : Ok(assignment);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Assign([FromBody] DeviceAssignmentRequestDto dto)
    {
        try
        {
            var created = await assignmentService.AssignAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/return")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ReturnDevice(int id)
    {
        var updated = await assignmentService.ReturnDeviceAsync(id);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await assignmentService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("self-assign")]
    [Authorize]
    public async Task<IActionResult> SelfAssign([FromBody] SelfAssignRequestDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var success = await assignmentService.SelfAssignAsync(dto.DeviceId, dto.Location, userId.Value);
        return success
            ? Ok(new { message = "Device assigned successfully." })
            : Conflict(new { message = "Device is already assigned to another user." });
    }

    [HttpDelete("self-unassign")]
    [Authorize]
    public async Task<IActionResult> SelfUnassign()
    {
        var userId = GetCurrentUserId();
        if (userId is null) return Unauthorized();

        var success = await assignmentService.SelfUnassignAsync(userId.Value);
        return success
            ? Ok(new { message = "Device unassigned successfully." })
            : BadRequest(new { message = "You have no device currently assigned." });
    }
}
