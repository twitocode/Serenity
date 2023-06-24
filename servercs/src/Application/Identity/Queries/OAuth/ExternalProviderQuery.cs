using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Serenity.Application.Interfaces;
using Serenity.Domain.Entities;
using Serenity.Application.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Serenity.Application.Identity.Queries.OAuth;

public record ExternalProviderQuery : IRequest<Result<string>>
{
    public string? ReturnUrl { get; set; }
    public string? RemoteError { get; set; }
}

public class ExternalProviderQueryHandler : IRequestHandler<ExternalProviderQuery, Result<string>>
{
    private readonly ILogger<ExternalProviderQuery> logger;
    private readonly SignInManager<ApplicationUser> signInManager;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IJwtService jwtService;

    public ExternalProviderQueryHandler(ILogger<ExternalProviderQuery> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IJwtService jwtService)
    {
        this.logger = logger;
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.jwtService = jwtService;
    }

    public async Task<Result<string>> Handle(ExternalProviderQuery request, CancellationToken cancellationToken)
    {
        if (request.RemoteError is not null)
        {
            logger.LogError("Error from external provider: {remoteError}", request.RemoteError);

            return Result<string>.ForError(StatusCodes.Status500InternalServerError, new List<ApplicationError> {
                new ApplicationError("", $"Error from external provider: {request.RemoteError}")
            });
        }

        var info = await signInManager.GetExternalLoginInfoAsync();

        if (info is null)
        {
            logger.LogError("Error loading external login info");

            return Result<string>.ForError(StatusCodes.Status500InternalServerError, new List<ApplicationError> {
                new ApplicationError("", $"Error loading external login info")
            });
        }

        var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
            info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        //If user does not exist
        //TODO - FIX LATER
        // if (!signInResult.Succeeded)
        // {
        //     logger.LogError("Could not log in with {Provider}", info.LoginProvider);

        //     return Result<string>.ForError(StatusCodes.Status500InternalServerError, new List<ApplicationError> {
        //         new ApplicationError("", $"Could not log in with {info.LoginProvider}")
        //     });
        // }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        if (email is null)
        {
            logger.LogError("Email not found in principal claim");

            return Result<string>.ForError(StatusCodes.Status400BadRequest, new List<ApplicationError> {
                new ApplicationError("", "Email not found in principal claim")
            });
        }

        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                Email = info.Principal.FindFirstValue(ClaimTypes.Email)
            };

            await userManager.CreateAsync(user);
        }

        string token = jwtService.CreateToken(user);

        await userManager.AddLoginAsync(user, info);
        await signInManager.SignInAsync(user, isPersistent: false);

        return new Result<string>
        {
            Data = $"Successfully logged in {user.Email}, {token}",
            Success = true,
            Errors = new List<ApplicationError> { }
        };
    }
}