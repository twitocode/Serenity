using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serenity.Application.Common.Models;
using Serenity.Application.Identity.Commands.Login.Local;
using Serenity.Application.Identity.Commands.Register.DisplayName;
using Serenity.Application.Identity.Commands.Register.Email;
using Serenity.Application.Identity.Commands.Register.OAuth;
using Serenity.Application.Identity.Commands.Register.UserDetails;
using Serenity.Application.Identity.Queries.OAuth;
using Serenity.Domain.Entities;
using Serenity.Web.Common;

namespace Serenity.Web.Controllers;

[Route("identity")]
public class IdentityController : ApiController {
	private readonly ILogger<IdentityController> logger;
	private readonly SignInManager<ApplicationUser> signInManager;

	public IdentityController(ILogger<IdentityController> logger, SignInManager<ApplicationUser> signInManager) {
		this.logger = logger;
		this.signInManager = signInManager;
	}

	[HttpGet("{provider}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
	public IActionResult ExternalProvider([FromRoute] string provider) {
		var redirectUrl = Url.Action("ExternalProviderCallback", "Identity");
		var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
		return new ChallengeResult(provider, properties);
	}

	[HttpGet("logout")]
	public IActionResult Logout() {
		HttpContext.SignOutAsync("Identity.Application");
		return LocalRedirect(Url.Content("~/"));
	}

	[HttpGet("external-callback")]
	public async Task<IActionResult> ExternalProviderCallback(string? returnUrl, string? remoteError) {
		var result = await Mediator.Send(new ExternalProviderQuery {
			RemoteError = remoteError,
			ReturnUrl = returnUrl ?? Url.Content("~/")
		});

		return StatusCode(result.StatusCode, result);
	}

	[HttpPatch("register/email")]
	public async Task<IActionResult> RegisterUserEmail([FromBody] EmailRegisterUserCommand command) {
		Result<string> result = await Mediator.Send(command);
		return StatusCode(result.StatusCode, result);
	}

	[HttpPatch("register/name")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public async Task<IActionResult> RegisterUserDisplayName([FromBody] DisplayNameRegisterUserCommand command) {
		command.Token = HttpContext.Request.Headers.Authorization[0]!["Bearer ".Length..];
		Result<string> result = await Mediator.Send(command);

		return StatusCode(result.StatusCode, result);
	}

	[HttpPatch("register/oauth")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public async Task<IActionResult> RegisterUserOauthDisplayName([FromBody] OAuthRegisterUserCommand command) {
		Result<string> result = await Mediator.Send(command);
		return StatusCode(result.StatusCode, result);
	}

	[HttpPatch("register/details")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public async Task<IActionResult> RegisterUserDetails([FromBody] UserDetailsRegisterUserCommand command) {
		command.Token = HttpContext.Request.Headers.Authorization[0]!["Bearer ".Length..];
		Result<string> result = await Mediator.Send(command);
		
		return StatusCode(result.StatusCode, result);
	}

	[HttpPost("login")]
	public async Task<IActionResult> LoginUser([FromBody] LocalLoginCommand command) {
		Result<string> result = await Mediator.Send(command);
		return StatusCode(result.StatusCode, result);
	}
}
