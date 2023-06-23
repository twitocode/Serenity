using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serenity.Api.Database.Entities;

namespace Serenity.Api.Modules.Identity;

[ApiController]
[Route("identity")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Identity")]
public class IdentityController : ControllerBase
{
    private readonly ILogger<IdentityController> logger;
    private readonly SignInManager<AppUser> signInManager;
    private readonly UserManager<AppUser> userManager;

    public IdentityController(ILogger<IdentityController> logger, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
    {
        this.logger = logger;
        this.signInManager = signInManager;
        this.userManager = userManager;
    }

    [HttpGet("access-dendied")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult AccessDendied()
    {
        return Ok("Access dendied");
    }

    [HttpGet("{provider}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    public IActionResult ExternalProvider([FromRoute] string provider)
    {
        var redirectUrl = Url.Action("ExternalProviderCallback", "Identity");
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    [HttpGet("external-callback")]
    public async Task<IActionResult> ExternalProviderCallback(string returnUrl = null, string remoteError = null)
    {
        string v = returnUrl ?? Url.Content("~/");
        returnUrl = v;

        if (remoteError is not null)
        {
            string err = $"Error from external provider: {remoteError}";
            logger.LogError(err);
            return BadRequest(err);
        }

        var info = await signInManager.GetExternalLoginInfoAsync();

        if (info is null)
        {
            string err = "Error loading external login info";
            logger.LogError(err);
            return BadRequest(err);
        }

        var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
               info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (signInResult.Succeeded)
        {
            return LocalRedirect(returnUrl);
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);

        if (email is null)
        {
            string err = "Email not found in principal claim";
            logger.LogError(err);
            return BadRequest(err);
        }

        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            logger.LogInformation(info.Principal.ToString());
            user = new AppUser
            {
                UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                Email = info.Principal.FindFirstValue(ClaimTypes.Email)
            };

            await userManager.CreateAsync(user);
        }

        // Add a login (i.e insert a row for the user in AspNetUserLogins table)
        await userManager.AddLoginAsync(user, info);
        await signInManager.SignInAsync(user, isPersistent: false);

        logger.LogInformation($"Successfully logged in {user.Email}");
        return Ok($"Successfully logged in {user.Email}");
    }
}
