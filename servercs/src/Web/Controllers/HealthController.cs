using Microsoft.AspNetCore.Mvc;
using Serenity.Web.Common;

namespace Serenity.Web.Controllers;

[Route("health")]
public class Health : ApiController {
	[HttpGet]
	public IActionResult RegisterUserDetails() {
		return Ok("Server is working");
	}
}
