using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Serenity.Web.Controllers;

[ApiController]
[Route("user")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "User")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> logger;
    public UserController(ILogger<UserController> logger)
    {
        this.logger = logger;
    }

    [HttpGet("protected")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult Protected()
    {

        return Ok("This is protected!");
    }
}
