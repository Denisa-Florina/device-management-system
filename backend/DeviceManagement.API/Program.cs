using DeviceManagement.Application;
using DeviceManagement.API.OpenApi;
using DeviceManagement.Infrastructure;
using DeviceManagement.Infrastructure.Data;
using DeviceManagement.Infrastructure.Identity;
using DeviceManagement.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DataSeeder.SeedAsync(
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(),
        scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(),
        scope.ServiceProvider.GetRequiredService<IUserRepository>());
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Device Management API";
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecuritySchemes = [JwtBearerDefaults.AuthenticationScheme]
        };
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
