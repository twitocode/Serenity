using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serenity.Application.Identity.Commands.Register.Email;
using Serenity.Application.Identity.Commands.Register.DisplayName;
using Serenity.Application.Identity.Commands.Register.UserDetails;
using Serenity.Application.Identity.Commands.Register.OAuth;

namespace Serenity.Application;

public static class ConfigureApplication {
	public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) {
		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

		services.AddScoped<IValidator<EmailRegisterUserCommand>, EmailRegisterUserCommandValidator>();
		services.AddScoped<IValidator<DisplayNameRegisterUserCommand>, DisplayNameRegisterUserCommandValidator>();
		services.AddScoped<IValidator<UserDetailsRegisterUserCommand>, UserDetailsRegisterUserCommandValidator>();
		services.AddScoped<IValidator<OAuthRegisterUserCommand>, OAuthRegisterUserCommandValidator>();

		return services;
	}
}
