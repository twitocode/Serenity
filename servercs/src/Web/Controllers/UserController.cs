using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Serenity.Web.Controllers;

[Route("user")]
public class UserController : ControllerBase {
	private readonly ILogger<UserController> logger;
	public UserController(ILogger<UserController> logger) {
		this.logger = logger;
	}

	[HttpGet("protected")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public IActionResult Protected() {

		return Ok("This is protected!");
	}
}
