using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Serenity.Application.Interfaces;
using Serenity.Domain.Entities;
using Serenity.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Serenity.Application.Identity.Queries.OAuth;

public record RegisterUserCommand : IRequest<Result<string>>
{
    public string Email { get; } = default!;
    public string DisplayName { get; } = default!;
    public string Password { get; } = default!;
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<string>>
{
    private readonly ILogger<RegisterUserCommand> logger;
    private readonly UserManager<AppUser> userManager;
    private readonly IJwtService jwtService;

    public RegisterUserCommandHandler(ILogger<RegisterUserCommand> logger, UserManager<AppUser> userManager, IJwtService jwtService)
    {
        this.logger = logger;
        this.userManager = userManager;
        this.jwtService = jwtService;
    }

    public async Task<Result<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is not null)
        {
            return Result<string>.ForError(new List<ApplicationError> {
                new ApplicationError("UserNotNullEmail", "User already exists with the given email")
            });
        }

        user = await userManager.FindByNameAsync(request.DisplayName);

        if (user is not null)
        {
            return Result<string>.ForError(new List<ApplicationError> {
                new ApplicationError("UserNotNullDisplayName", "User already exists with the given DisplayName")
            });
        }

        user = new AppUser
        {
            UserName = request.DisplayName,
            Email = request.Email,
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return Result<string>.ForError(result.Errors.Select(e => new ApplicationError(e.Code, e.Description)).ToList());
        }

        string token = jwtService.CreateToken(user);

        return new Result<string>
        {
            Data = token,
            Success = true,
            Errors = new List<ApplicationError> { }
        };
    }
}