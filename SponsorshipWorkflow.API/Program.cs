using SponsorshipWorkflow.API.Extensions;
using SponsorshipWorkflow.API.Middleware;
using SponsorshipWorkflow.Infrastructure.Extensions;
using SponsorshipWorkflow.Infrastructure.Data;
using SponsorshipWorkflow.Infrastructure.Services;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage))
                .ToList();

            var response = new
            {
                success = false,
                statusCode = 422,
                message = "Validation failed.",
                errors,
                timestamp = DateTime.UtcNow,
                traceId = context.HttpContext.TraceIdentifier
            };

            return new Microsoft.AspNetCore.Mvc.UnprocessableEntityObjectResult(response);
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// â”€â”€ Auto migrate and seed database â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

    db.Database.Migrate();
    db.Database.ExecuteSqlRaw("""
        IF NOT EXISTS (
            SELECT 1
            FROM sys.indexes
            WHERE name = N'IX_RefreshTokens_UserId_IsRevoked'
              AND object_id = OBJECT_ID(N'[dbo].[RefreshTokens]')
        )
        CREATE INDEX IX_RefreshTokens_UserId_IsRevoked
        ON [dbo].[RefreshTokens] ([UserId], [IsRevoked]);
        """);

    // Ensure core roles exist. Do not depend on an empty Roles table only.
    var requiredRoles = new[]
    {
        new Role { Name = "Requestor", Description = "Can submit and manage own sponsorship requests" },
        new Role { Name = "Manager", Description = "Can approve or reject requests at manager level" },
        new Role { Name = "FinanceAdmin", Description = "Can perform final approval or rejection" },
        new Role { Name = "SystemAdmin", Description = "Has full system access" }
    };

    foreach (var role in requiredRoles)
    {
        var existingRole = db.Roles.FirstOrDefault(r => r.Name == role.Name);
        if (existingRole is null)
        {
            db.Roles.Add(role);
        }
        else
        {
            existingRole.Description = role.Description;
            existingRole.IsActive = true;
        }
    }

    db.SaveChanges();

    // Seed sponsorship types if empty
    if (!db.SponsorshipTypes.Any())
    {
        db.SponsorshipTypes.AddRange(
            new SponsorshipType { Name = "Corporate Event", Description = "Corporate events and conferences" },
            new SponsorshipType { Name = "Sports Event", Description = "Sports tournaments and activities" },
            new SponsorshipType { Name = "Community Outreach", Description = "Community programs and CSR" },
            new SponsorshipType { Name = "Education & Training", Description = "Educational programs" },
            new SponsorshipType { Name = "Cultural Event", Description = "Cultural and arts events" },
            new SponsorshipType { Name = "Charity & Fundraising", Description = "Charitable causes" },
            new SponsorshipType { Name = "Industry Conference", Description = "Industry conferences and expos" },
            new SponsorshipType { Name = "Internal Team Event", Description = "Internal team building" }
        );
        db.SaveChanges();
    }

    const string adminEmail = "admin@sponsorship.com";
    const string adminPassword = "Admin@2026";
    const string seedUserPassword = "Admin@1234";

    var adminRole = db.Roles.First(r => r.Name == "SystemAdmin");
    var managerRole = db.Roles.First(r => r.Name == "Manager");
    var financeRole = db.Roles.First(r => r.Name == "FinanceAdmin");
    var requestorRole = db.Roles.First(r => r.Name == "Requestor");

    // Always repair the admin account on startup so stale database rows cannot break admin login.
    var admin = db.Users.FirstOrDefault(u => u.Email.ToLower() == adminEmail);
    var (adminHash, adminSalt) = passwordService.HashPassword(adminPassword);
    if (admin is null)
    {
        db.Users.Add(new User
        {
            Id = Guid.Parse("A1000000-0000-0000-0000-000000000001"),
            FullName = "System Administrator",
            Email = adminEmail,
            Department = "IT",
            PasswordHash = adminHash,
            PasswordSalt = adminSalt,
            RoleId = adminRole.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
    }
    else
    {
        admin.FullName = "System Administrator";
        admin.Department = "IT";
        admin.PasswordHash = adminHash;
        admin.PasswordSalt = adminSalt;
        admin.RoleId = adminRole.Id;
        admin.IsActive = true;
        admin.UpdatedAt = DateTime.UtcNow;
    }

    var (seedHash, seedSalt) = passwordService.HashPassword(seedUserPassword);
    var seedUsers = new[]
    {
        new User
        {
            Id = Guid.Parse("A2000000-0000-0000-0000-000000000002"),
            FullName = "James Wilson",
            Email = "manager@sponsorship.com",
            Department = "Operations",
            PasswordHash = seedHash,
            PasswordSalt = seedSalt,
            RoleId = managerRole.Id,
            IsActive = true
        },
        new User
        {
            Id = Guid.Parse("A3000000-0000-0000-0000-000000000003"),
            FullName = "Sarah Finance",
            Email = "finance@sponsorship.com",
            Department = "Finance",
            PasswordHash = seedHash,
            PasswordSalt = seedSalt,
            RoleId = financeRole.Id,
            IsActive = true
        },
        new User
        {
            Id = Guid.Parse("A4000000-0000-0000-0000-000000000004"),
            FullName = "John Requestor",
            Email = "requestor@sponsorship.com",
            Department = "Marketing",
            PasswordHash = seedHash,
            PasswordSalt = seedSalt,
            RoleId = requestorRole.Id,
            IsActive = true
        },
        new User
        {
            Id = Guid.Parse("A5000000-0000-0000-0000-000000000005"),
            FullName = "Naimul Islam",
            Email = "naim@sponsorship.com",
            Department = "Business Development",
            PasswordHash = seedHash,
            PasswordSalt = seedSalt,
            RoleId = requestorRole.Id,
            IsActive = true
        }
    };

    foreach (var seedUser in seedUsers)
    {
        if (!db.Users.Any(u => u.Email.ToLower() == seedUser.Email))
        {
            db.Users.Add(seedUser);
        }
    }

    db.SaveChanges();
}
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sponsorship Workflow API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", () => new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
});

app.Run();
