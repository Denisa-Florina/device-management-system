using DeviceManagement.Domain.Entities;
using DeviceManagement.Domain.Interfaces;
using DeviceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagement.Infrastructure.Repositories;

public class DeviceRepository(ApplicationDbContext context) : IDeviceRepository
{
    public async Task<IEnumerable<Device>> GetAllAsync() =>
        await context.Devices.ToListAsync();

    public async Task<Device?> GetByIdAsync(int id) =>
        await context.Devices.FindAsync(id);

    public async Task<Device> CreateAsync(Device device)
    {
        context.Devices.Add(device);
        await context.SaveChangesAsync();
        return device;
    }

    public async Task<Device> UpdateAsync(Device device)
    {
        context.Devices.Update(device);
        await context.SaveChangesAsync();
        return device;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var deleted = await context.Devices.Where(d => d.Id == id).ExecuteDeleteAsync();
        return deleted > 0;
    }
}
