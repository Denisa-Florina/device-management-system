namespace DeviceManagement.Application.DTOs.Auth;

public class SelfAssignRequestDto
{
    public int DeviceId { get; set; }
    public string Location { get; set; } = string.Empty;
}
