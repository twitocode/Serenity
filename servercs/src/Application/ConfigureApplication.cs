using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serenity.Application.Identity.Commands.Register.Email;
using Serenity.Application.Identity.Commands.Register.DisplayName;
using Serenity.Application.Identity.Commands.Register.UserDetails;
using Serenity.Application.Identity.Commands.Register.OAuth;
using Serenity.Application.Identity.Commands.Login.Local;

namespace Serenity.Application;

public static class ConfigureApplication {
	public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) {
		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
		services.AddTransient<IValidator<EmailRegisterUserCommand>, EmailRegisterUserCommandValidator>();
		services.AddTransient<IValidator<DisplayNameRegisterUserCommand>, DisplayNameRegisterUserCommandValidator>();
		services.AddTransient<IValidator<UserDetailsRegisterUserCommand>, UserDetailsRegisterUserCommandValidator>();
		services.AddTransient<IValidator<OAuthRegisterUserCommand>, OAuthRegisterUserCommandValidator>();
		services.AddTransient<IValidator<LocalLoginCommand>, LocalLoginCommandValidator>();
		
		return services;
	}
}
