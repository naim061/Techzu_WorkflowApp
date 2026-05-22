using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SponsorshipWorkflow.Application.Interfaces;
using SponsorshipWorkflow.Application.Services;
using SponsorshipWorkflow.Domain.Interfaces;
using SponsorshipWorkflow.Infrastructure.Data;
using SponsorshipWorkflow.Infrastructure.Services;

namespace SponsorshipWorkflow.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(3);
                    sqlOptions.CommandTimeout(30);
                }));

        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISponsorshipRequestService, SponsorshipRequestService>();
        services.AddScoped<ISponsorshipTypeService, SponsorshipTypeService>();

        return services;
    }
}