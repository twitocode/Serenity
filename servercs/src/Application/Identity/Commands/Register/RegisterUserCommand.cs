using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Serenity.Application.Interfaces;
using Serenity.Domain.Entities;
using Serenity.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using FluentValidation.Results;

namespace Serenity.Application.Identity.Queries.OAuth;

public record RegisterUserCommand : IRequest<Result<string>>
{
    public string Email { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<string>>
{
    private readonly ILogger<RegisterUserCommand> logger;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IJwtService jwtService;
    private readonly IValidator<RegisterUserCommand> validator;

    public RegisterUserCommandHandler(ILogger<RegisterUserCommand> logger, UserManager<ApplicationUser> userManager, IJwtService jwtService, IValidator<RegisterUserCommand> validator)
    {
        this.logger = logger;
        this.userManager = userManager;
        this.jwtService = jwtService;
        this.validator = validator;
    }

    public async Task<Result<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        ValidationResult valResult = await validator.ValidateAsync(request, cancellationToken);

        if (!valResult.IsValid)
        {
            return Result<string>.ForError(StatusCodes.Status400BadRequest, valResult.Errors.Select(e => new ApplicationError(e.PropertyName, e.ErrorMessage)).ToList());
        }

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is not null)
        {
            return Result<string>.ForError(StatusCodes.Status400BadRequest, new List<ApplicationError> {
                new ApplicationError("UserNotNullEmail", "User already exists with the given email")
            });
        }

        user = await userManager.FindByNameAsync(request.DisplayName);

        if (user is not null)
        {
            return Result<string>.ForError(StatusCodes.Status400BadRequest, new List<ApplicationError> {
                new ApplicationError("UserNotNullDisplayName", "User already exists with the given DisplayName")
            });
        }

        user = new ApplicationUser
        {
            UserName = request.DisplayName,
            Email = request.Email,
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return Result<string>.ForError(StatusCodes.Status500InternalServerError, result.Errors.Select(e => new ApplicationError(e.Code, e.Description)).ToList());
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