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
using Serenity.Domain.ValueObjects;

namespace Serenity.Application.Identity.Commands.Register.UserDetails;

public record UserDetailsRegisterUserCommand : IRequest<Result<string>> {
	public string Email { get; set; } = default!;
	public string AvatarUrl { get; set; } = default!;
	public string Pronouns { get; set; } = default!;
	public string Gender { get; set; } = default!;
}

public class UserDetailsRegisterUserCommandHandler : IRequestHandler<UserDetailsRegisterUserCommand, Result<string>> {
	private readonly ILogger<UserDetailsRegisterUserCommand> logger;
	private readonly UserManager<ApplicationUser> userManager;
	private readonly IJwtService jwtService;
	private readonly IValidator<UserDetailsRegisterUserCommand> validator;
	private readonly IDataContext dataContext;
	private readonly ICacheService cache;

	public UserDetailsRegisterUserCommandHandler(
		ILogger<UserDetailsRegisterUserCommand> logger,
		UserManager<ApplicationUser> userManager,
		IJwtService jwtService,
		IValidator<UserDetailsRegisterUserCommand> validator,
		IDataContext dataContext,
		ICacheService cache
	) {
		this.logger = logger;
		this.userManager = userManager;
		this.jwtService = jwtService;
		this.validator = validator;
		this.dataContext = dataContext;
		this.cache = cache;
	}

	public async Task<Result<string>> Handle(UserDetailsRegisterUserCommand request, CancellationToken cancellationToken) {
		ValidationResult valResult = await validator.ValidateAsync(request, cancellationToken);

		if (!valResult.IsValid) {
			var errors = valResult.Errors
				.Select(e => new ApplicationError(e.PropertyName, e.ErrorMessage))
				.ToList();

			return Result<string>.ForError(StatusCodes.Status400BadRequest, errors);
		}

		var cachedUser = cache.GetData<RedisUser>($"create-user:{request.Email}");

		//User is already in cache
		if (cachedUser is null) {
			return Result<string>.ForError(StatusCodes.Status500InternalServerError, new List<ApplicationError> {
				new ApplicationError("UserNotInCache", "User does not exists in cache")
			});
		}

		IdentityResult result;
		ApplicationUser? user;

		if (cachedUser.StringId is not null) {
			user = await userManager.FindByIdAsync(cachedUser.StringId);

			if (user is not null) {
				user.AvatarUrl = request.AvatarUrl;
				user.PersonDetails = new PersonDetails(request.Gender, request.Pronouns);
				user.Stats = new UserStats { };
				user.UserName = cachedUser.UserName;

				result = await userManager.UpdateAsync(user);
			} else {
				return Result<string>.ForError(StatusCodes.Status500InternalServerError, new List<ApplicationError> {
					new ApplicationError("UserNotFound", "User in cache does not exist in database")
				});
			}
		} else {
			//User is using a password
			user = new ApplicationUser {
				Id = Guid.NewGuid(),
				UserName = cachedUser.UserName,
				Email = cachedUser.Email,
				AvatarUrl = request.AvatarUrl,
				BackgroundUrl = null,
				PasswordLock = null,
				PersonDetails = new PersonDetails(request.Gender, request.Pronouns),
				Stats = new UserStats { }
			};

			result = await userManager.CreateAsync(user, cachedUser.Password);
		}

		if (!result.Succeeded) {
			return Result<string>.ForError(StatusCodes.Status500InternalServerError, result.Errors.Select(e => new ApplicationError(e.Code, e.Description)).ToList());
		}

		bool removed = cache.RemoveData($"create-user:{user.Email}");

		if (!removed) {
			return Result<string>.ForError(StatusCodes.Status500InternalServerError, new List<ApplicationError> {
				new ApplicationError("RemoveUserFromCache", "Created user could not be removed from cache")
			});
		}

		string token = jwtService.CreateToken(user);
		return Result<string>.ForSuccess(token);
	}
}
