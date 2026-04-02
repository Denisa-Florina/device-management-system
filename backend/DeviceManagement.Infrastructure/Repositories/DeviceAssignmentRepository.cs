using DeviceManagement.Domain.Entities;
using DeviceManagement.Domain.Interfaces;
using DeviceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagement.Infrastructure.Repositories;

public class DeviceAssignmentRepository(ApplicationDbContext context) : IDeviceAssignmentRepository
{
    public async Task<IEnumerable<DeviceAssignment>> GetAllAsync() =>
        await context.DeviceAssignments
            .Include(a => a.Device)
            .Include(a => a.User)
            .ToListAsync();

    public async Task<DeviceAssignment?> GetByIdAsync(int id) =>
        await context.DeviceAssignments
            .Include(a => a.Device)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);



    public async Task<DeviceAssignment> CreateAsync(DeviceAssignment assignment)
    {
        context.DeviceAssignments.Add(assignment);
        await context.SaveChangesAsync();
        return assignment;
    }

    public async Task<DeviceAssignment> UpdateAsync(DeviceAssignment assignment)
    {
        context.DeviceAssignments.Update(assignment);
        await context.SaveChangesAsync();
        return assignment;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var deleted = await context.DeviceAssignments.Where(a => a.Id == id).ExecuteDeleteAsync();
        return deleted > 0;
    }

	public async Task<DeviceAssignment?> GetCurrentAssignmentForDeviceAsync(int deviceId) =>
        await context.DeviceAssignments
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.DeviceId == deviceId && a.ReturnedDate == null);
}
