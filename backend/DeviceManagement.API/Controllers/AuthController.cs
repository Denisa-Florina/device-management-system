using DeviceManagement.Application.DTOs.Auth;
using DeviceManagement.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagement.API.Controllers;

public class AuthController(IAuthService authService) : BaseController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
    {
        try
        {
            var result = await authService.RegisterAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await authService.LoginAsync(dto);
        return result is null
            ? Unauthorized(new { message = "Invalid email or password." })
            : Ok(result);
    }
}
