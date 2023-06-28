using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Serenity.Application.Interfaces;
using Serenity.Infrastructure.Services;
using Serenity.Domain.Entities;
using Serenity.Infrastructure.Persistence;

namespace Serenity.Infrastructure;

public static class ConfigureInfrastructure {
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config) {
		var dataSourceBuilder = new NpgsqlDataSourceBuilder(config.GetConnectionString("DB"));
		dataSourceBuilder.UseNodaTime();
		var dataSource = dataSourceBuilder.Build();

		services.AddDbContext<DataContext>(options =>
			options.UseNpgsql(dataSource, o => o.UseNodaTime())
		);

		services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options => {
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
		services.AddTransient<IDataContext, DataContext>();
		services.AddSingleton<ICacheService, RedisService>();

		return services;
	}
}
