using DeviceManagement.Domain.Entities;
using DeviceManagement.Domain.Interfaces;
using DeviceManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagement.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllAsync() =>
        await context.Users.ToListAsync();

    public async Task<User?> GetByIdAsync(int id) =>
        await context.Users.FindAsync(id);

    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var deleted = await context.Users.Where(u => u.Id == id).ExecuteDeleteAsync();
        return deleted > 0;
    }
}
