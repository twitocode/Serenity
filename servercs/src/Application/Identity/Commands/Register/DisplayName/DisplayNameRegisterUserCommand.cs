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

namespace Serenity.Application.Identity.Commands.Register.DisplayName;

public record DisplayNameRegisterUserCommand : IRequest<Result<string>> {
	public string? DisplayName { get; set; } = default;
	public string? Password { get; set; } = default;
	public string? Token { get; set; } = default;
}

public class DisplayNameRegisterUserCommandHandler : IRequestHandler<DisplayNameRegisterUserCommand, Result<string>> {
	private readonly ILogger<DisplayNameRegisterUserCommand> logger;
	private readonly UserManager<ApplicationUser> userManager;
	private readonly IJwtService jwtService;
	private readonly IValidator<DisplayNameRegisterUserCommand> validator;
	private readonly ICacheService cache;

	public DisplayNameRegisterUserCommandHandler(
		ILogger<DisplayNameRegisterUserCommand> logger,
		UserManager<ApplicationUser> userManager,
		IJwtService jwtService,
		IValidator<DisplayNameRegisterUserCommand> validator,
		ICacheService cache
	) {
		this.logger = logger;
		this.userManager = userManager;
		this.jwtService = jwtService;
		this.validator = validator;
		this.cache = cache;
	}

	public async Task<Result<string>> Handle(DisplayNameRegisterUserCommand request, CancellationToken cancellationToken) {
		ValidationResult valResult = await validator.ValidateAsync(request, cancellationToken);

		if (!valResult.IsValid) {
			var errors = valResult.Errors
				.Select(e => new ApplicationError(e.PropertyName, e.ErrorMessage))
				.ToList();

			return Result<string>.ForError(StatusCodes.Status400BadRequest, errors);
		}

		var tokenUser = jwtService.ReadToken(request.Token!);
		var cachedUser = cache.GetData<RedisUser>($"create-user:{tokenUser.Email}");

		//User is already in cache
		if (cachedUser is null) {
			return Result<string>.ForError(StatusCodes.Status500InternalServerError, new List<ApplicationError> {
				new ApplicationError("UserNotInCache", "User does not exists in cache")
			});
		}

		var user = await userManager.FindByNameAsync(request.DisplayName!);

		if (user is not null) {
			return Result<string>.ForError(StatusCodes.Status400BadRequest, new List<ApplicationError> {
				new ApplicationError("UserNotNullDisplayName", "User already exists with the given display name")
			});
		}

		var passwordValidator = new PasswordValidator<ApplicationUser>();
		var result = await passwordValidator.ValidateAsync(userManager, null, request.Password);

		if (!result.Succeeded) {
			var errors = result.Errors.Select(e => new ApplicationError(e.Code, e.Description)).ToList();
			return Result<string>.ForError(400, errors);
		}

		cachedUser.UserName = request.DisplayName;
		cachedUser.Password = request.Password;

		bool isSet = cache.SetData($"create-user:{cachedUser.Email}", cachedUser, cachedUser.ExpirationTime);

		if (!isSet) {
			return Result<string>.ForError(StatusCodes.Status500InternalServerError, new List<ApplicationError> {
				new ApplicationError("UserNotSetInCache", "Could not set user in cache")
			});
		}

		string token = jwtService.CreateToken(cachedUser);
		return Result<string>.ForSuccess(200, token);
	}
}
