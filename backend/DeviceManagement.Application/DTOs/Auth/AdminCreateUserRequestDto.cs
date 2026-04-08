namespace DeviceManagement.Application.DTOs.Auth;

public class AdminCreateUserRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer";
}
