using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Serenity.Application.Interfaces;
using Serenity.Domain.Entities;
using Serenity.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using FluentValidation.Results;
using Serenity.Domain.Entities.Redis;

namespace Serenity.Application.Identity.Commands.Register.Email;

public record EmailRegisterUserCommand : IRequest<Result<string>> {
	public string Email { get; set; } = default!;
}

public class EmailRegisterUserCommandHandler : IRequestHandler<EmailRegisterUserCommand, Result<string>> {
	private readonly ILogger<EmailRegisterUserCommand> logger;
	private readonly UserManager<ApplicationUser> userManager;
	private readonly IJwtService jwtService;
	private readonly IValidator<EmailRegisterUserCommand> validator;
	private readonly ICacheService cache;

	public EmailRegisterUserCommandHandler(
		ILogger<EmailRegisterUserCommand> logger,
		UserManager<ApplicationUser> userManager,
		IJwtService jwtService,
		IValidator<EmailRegisterUserCommand> validator,
		ICacheService cache
	) {
		this.logger = logger;
		this.userManager = userManager;
		this.jwtService = jwtService;
		this.validator = validator;
		this.cache = cache;
	}

	public async Task<Result<string>> Handle(EmailRegisterUserCommand request, CancellationToken cancellationToken) {
		ValidationResult valResult = await validator.ValidateAsync(request, cancellationToken);

		if (!valResult.IsValid) {
			var errors = valResult.Errors
				.Select(e => new ApplicationError(e.PropertyName, e.ErrorMessage))
				.ToList();

			return Result<string>.ForError(StatusCodes.Status400BadRequest, errors);
		}

		var user = await userManager.FindByEmailAsync(request.Email);
		if (user is not null) {
			return Result<string>.ForError(StatusCodes.Status400BadRequest, new List<ApplicationError> {
				new ApplicationError("UserNotNullEmail", "User already exists with the given email")
			});
		}

		var cachedUser = cache.GetData<RedisUser>($"create-user:{request.Email}");

		//User is already in cache
		if (cachedUser is not null) {
			///I dunno
		}

		var offset = DateTimeOffset.Now.AddDays(1);

		cachedUser = new RedisUser {
			Email = request.Email,
			ExpirationTime = offset
		};

		bool isSet = cache.SetData($"create-user:{request.Email}", cachedUser, offset);

		if (!isSet) {
			return Result<string>.ForError(StatusCodes.Status500InternalServerError, new List<ApplicationError> {
				new ApplicationError("UserNotSetInCache", "Could not set user in cache")
			});
		}

		string token = jwtService.CreateToken(cachedUser);
		return Result<string>.ForSuccess(token);
	}
}
