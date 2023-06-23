using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serenity.Application.Interfaces;
using Serenity.Application.Services;
using Serenity.Domain.Entities;

namespace Serenity.Infrastructure;

public static class ConfigureInfrastructure
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DB"))
        );

        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;

            options.Lockout.MaxFailedAccessAttempts = 10;
        })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

        services.AddSingleton<IJwtService, JwtService>();

        return services;
    }
}
