using DeviceManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagement.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<User> Users => Set<User>();
    public DbSet<DeviceAssignment> DeviceAssignments => Set<DeviceAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
            entity.Property(d => d.Manufacturer).IsRequired().HasMaxLength(100);
            entity.Property(d => d.OperatingSystem).IsRequired().HasMaxLength(100);
            entity.Property(d => d.OSVersion).IsRequired().HasMaxLength(100);
            entity.Property(d => d.Processor).IsRequired().HasMaxLength(100);
            entity.Property(d => d.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Role).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Location).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<DeviceAssignment>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Location).IsRequired().HasMaxLength(100);

            entity.HasOne(a => a.Device)
                .WithMany(d => d.Assignments)
                .HasForeignKey(a => a.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.User)
                .WithMany(u => u.Assignments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
