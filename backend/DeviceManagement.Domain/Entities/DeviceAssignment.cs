namespace DeviceManagement.Domain.Entities;

public class DeviceAssignment
{
    public int Id { get; set; }
    public int DeviceId { get; set; }
    public Device Device { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Location { get; set; } = string.Empty;
    public DateTime AssignedDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
}
