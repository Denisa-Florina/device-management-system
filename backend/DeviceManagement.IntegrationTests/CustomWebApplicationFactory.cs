using DeviceManagement.Application.Services.Interfaces;
using DeviceManagement.Infrastructure.Data;
using DeviceManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceManagement.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "TestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbDescriptors = services
                .Where(d =>
                    d.ServiceType == typeof(ApplicationDbContext) ||
                    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    (d.ServiceType.IsGenericType &&
                     d.ServiceType.GetGenericTypeDefinition().FullName?.Contains("IDbContextOptionsConfiguration") == true) ||
                    d.ServiceType.FullName?.Contains("SqlServer") == true ||
                    d.ImplementationType?.FullName?.Contains("SqlServer") == true)
                .ToList();
            foreach (var d in dbDescriptors) services.Remove(d);

            var dbName = _dbName;
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(dbName));

            var aiDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IAIDescriptionService));
            if (aiDescriptor != null) services.Remove(aiDescriptor);
            services.AddSingleton<IAIDescriptionService, FakeAIDescriptionService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
        });

        builder.UseEnvironment("Testing");
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync(string role = "Admin")
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Role", role);
        client.DefaultRequestHeaders.Add("X-Test-UserId", "1");

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.EnsureCreatedAsync();
        await DataSeeder.SeedAsync(roleManager, userManager,
            scope.ServiceProvider.GetRequiredService<DeviceManagement.Domain.Interfaces.IUserRepository>());

        return client;
    }
}
