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

namespace Serenity.Application.Identity.Commands.Register.OAuth;

public record OAuthRegisterUserCommand : IRequest<Result<string>> {
	public string DisplayName { get; set; } = default!;
	public string? Token { get; set; } = default;
}

public class OAuthRegisterUserCommandHandler : IRequestHandler<OAuthRegisterUserCommand, Result<string>> {
	private readonly ILogger<OAuthRegisterUserCommand> logger;
	private readonly UserManager<ApplicationUser> userManager;
	private readonly IJwtService jwtService;
	private readonly IValidator<OAuthRegisterUserCommand> validator;
	private readonly ICacheService cache;

	public OAuthRegisterUserCommandHandler(
		ILogger<OAuthRegisterUserCommand> logger,
		UserManager<ApplicationUser> userManager,
		IJwtService jwtService,
		IValidator<OAuthRegisterUserCommand> validator,
		ICacheService cache
	) {
		this.logger = logger;
		this.userManager = userManager;
		this.jwtService = jwtService;
		this.validator = validator;
		this.cache = cache;
	}

	public async Task<Result<string>> Handle(OAuthRegisterUserCommand request, CancellationToken cancellationToken) {
		ValidationResult valResult = await validator.ValidateAsync(request, cancellationToken);

		if (!valResult.IsValid) {
			var errors = valResult.Errors
				.Select(e => new ApplicationError(e.PropertyName, e.ErrorMessage))
				.ToList();

			return Result<string>.ForError(StatusCodes.Status400BadRequest, errors);
		}
		var user = jwtService.ReadToken(request.Token!);
		var oauthUser = await userManager.FindByIdAsync(user.Id.ToString());

		if (oauthUser is null) {
			return Result<string>.ForError(StatusCodes.Status500InternalServerError, new List<ApplicationError> {
				new ApplicationError("OauthUserNotFound", "User does not exist with Oauth")
			});
		}

		var cachedUser = cache.GetData<RedisUser>($"create-user:{oauthUser.Email}");

		//User is not already in cache
		if (cachedUser is null) {
			var offset = DateTimeOffset.Now.AddDays(1);

			cachedUser = new RedisUser{
				Email = oauthUser.Email,
				ExpirationTime = offset,
				StringId = oauthUser.Id.ToString()
			};

			bool isCacheSet = cache.SetData($"create-user:{oauthUser.Email}", cachedUser, offset);
		}

		var foundUser = await userManager.FindByNameAsync(request.DisplayName);

		if (foundUser is not null) {
			return Result<string>.ForError(StatusCodes.Status400BadRequest, new List<ApplicationError> {
				new ApplicationError("UserNotNullDisplayName", "User already exists with the given display name")
			});
		}

		cachedUser.UserName = request.DisplayName;
		cachedUser.Email = oauthUser.Email;

		bool isSet = cache.SetData($"create-user:{oauthUser.Email}", cachedUser, cachedUser.ExpirationTime);

		if (!isSet) {
			return Result<string>.ForError(StatusCodes.Status500InternalServerError, new List<ApplicationError> {
				new ApplicationError("UserNotSetInCache", "Could not set user in cache")
			});
		}

		string token = jwtService.CreateToken(cachedUser);
		return Result<string>.ForSuccess(201, token);
	}
}
