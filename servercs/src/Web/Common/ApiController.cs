using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Serenity.Web.Common;

[ApiController]
[Produces("application/json")]
public class ApiController : ControllerBase {
	private IMediator _mediator = null!;
	protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}
