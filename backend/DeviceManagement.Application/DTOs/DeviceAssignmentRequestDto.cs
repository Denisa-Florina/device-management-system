namespace DeviceManagement.Application.DTOs;

public class DeviceAssignmentRequestDto
{
    public int DeviceId { get; set; }
    public int UserId { get; set; }
    public string Location { get; set; } = string.Empty;
}
