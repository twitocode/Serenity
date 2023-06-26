using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Serenity.Web;

public static class ConfigureWeb {
	public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration config) {
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
		options.TokenValidationParameters = new TokenValidationParameters {
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidIssuer = config["Authentication:JwtSettings:Issuer"],
			ValidAudience = config["Authentication:JwtSettings:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(config["Authentication:JwtSettings:Key"] ?? "")
			),
		};
	})
	.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
		// Cookie settings
		options.Cookie.HttpOnly = true;
		options.ExpireTimeSpan = TimeSpan.FromDays(7);

		// options.LoginPath = "/identity/login";
		// options.AccessDeniedPath = "/identity/access-denied";
		options.SlidingExpiration = true;
	})
	.AddGoogle(options => {
		options.ClientId = config["Authentication:Google:ClientId"] ?? "";
		options.ClientSecret = config["Authentication:Google:ClientSecret"] ?? "";
	})
	.AddGitHub(options => {
		options.ClientId = config["Authentication:Github:ClientId"] ?? "";
		options.ClientSecret = config["Authentication:Github:ClientSecret"] ?? "";
		options.Scope.Add("user:email");
	})
	.AddDiscord(options => {
		options.ClientId = config["Authentication:Discord:ClientId"] ?? "";
		options.ClientSecret = config["Authentication:Discord:ClientSecret"] ?? "";
		options.Scope.Add("email");
	});

		services.Configure<CookiePolicyOptions>(options => {
			options.Secure = CookieSecurePolicy.Always;
		});

		return services;
	}
}
