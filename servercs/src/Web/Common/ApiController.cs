using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Serenity.Web.Common;

public class ApiController : ControllerBase {
    private IMediator _mediator = null!;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}