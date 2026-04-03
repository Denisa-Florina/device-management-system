using DeviceManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DeviceManagement.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public int? UserId { get; set; }
    public User? User { get; set; }
}
