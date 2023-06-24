using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serenity.Application.Common.Models;
using Serenity.Application.Identity.Queries.OAuth;
using Serenity.Application.Interfaces;
using Serenity.Domain.Entities;
using Serenity.Web.Common;

namespace Serenity.Controllers;

[ApiController]
[Route("identity")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Identity")]
public class IdentityController : ApiController
{
    private readonly ILogger<IdentityController> logger;
    private readonly SignInManager<ApplicationUser> signInManager;

    public IdentityController(ILogger<IdentityController> logger, SignInManager<ApplicationUser> signInManager)
    {
        this.logger = logger;
        this.signInManager = signInManager;
    }

    [HttpGet("{provider}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult ExternalProvider([FromRoute] string provider)
    {
        var redirectUrl = Url.Action("ExternalProviderCallback", "Identity");
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.SignOutAsync("Identity.Application");
        return LocalRedirect(Url.Content("~/"));
    }

    [HttpGet("external-callback")]
    public async Task<IActionResult> ExternalProviderCallback(string? returnUrl, string? remoteError)
    {
        var result = await Mediator.Send(new ExternalProviderQuery
        {
            RemoteError = remoteError,
            ReturnUrl = returnUrl ?? Url.Content("~/")
        });

        return StatusCode(result.StatusCode, result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand command)
    {
        Result<string> result = await Mediator.Send(command);
        return StatusCode(result.StatusCode, result);
    }
}
