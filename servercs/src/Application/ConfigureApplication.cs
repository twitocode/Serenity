using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serenity.Application.Identity.Commands.Register;
using Serenity.Application.Identity.Queries.OAuth;

namespace Serenity.Application;

public static class ConfigureApplication
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped<IValidator<RegisterUserCommand>, RegisterUserCommandValidator>();

        return services;
    }
}
