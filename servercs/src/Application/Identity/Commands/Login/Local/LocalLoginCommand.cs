using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Serenity.Application.Interfaces;
using Serenity.Domain.Entities;
using Serenity.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using FluentValidation.Results;

namespace Serenity.Application.Identity.Commands.Login.Local;

public record LocalLoginCommand : IRequest<Result<string>> {
	public string Password { get; set; } = default;
	public string Identifier { get; set; } = default;
}

public class LocalLoginCommandHandler : IRequestHandler<LocalLoginCommand, Result<string>> {
	private readonly ILogger<LocalLoginCommand> logger;
	private readonly UserManager<ApplicationUser> userManager;
	private readonly IJwtService jwtService;
	private readonly IValidator<LocalLoginCommand> validator;
	private readonly SignInManager<ApplicationUser> signInManager;

	public LocalLoginCommandHandler(
		ILogger<LocalLoginCommand> logger,
		UserManager<ApplicationUser> userManager,
		IJwtService jwtService,
		IValidator<LocalLoginCommand> validator,
		SignInManager<ApplicationUser> signInManager
	) {
		this.logger = logger;
		this.userManager = userManager;
		this.jwtService = jwtService;
		this.validator = validator;
		this.signInManager = signInManager;
	}

	public async Task<Result<string>> Handle(LocalLoginCommand request, CancellationToken cancellationToken) {
		logger.LogInformation("{@request}", request);
		ValidationResult valResult = await validator.ValidateAsync(request, cancellationToken);

		if (!valResult.IsValid) {
			var errors = valResult.Errors
				.Select(e => new ApplicationError(e.PropertyName, e.ErrorMessage))
				.ToList();

			return Result<string>.ForError(StatusCodes.Status400BadRequest, errors);
		}

		var user = await userManager.FindByEmailAsync(request.Identifier);

		if (user is null) {
			user = await userManager.FindByNameAsync(request.Identifier);

			if (user is null) {
				return Result<string>.ForError(StatusCodes.Status400BadRequest, new List<ApplicationError> {
					new ApplicationError("UserNotFound", "User does not exists in the database")
				});
			}

			logger.LogInformation("User exists with username {@name}", request.Identifier);
		} else {
			logger.LogInformation("User exists with email {@email}", request.Identifier);
		}

		if (string.IsNullOrEmpty(user.PasswordHash)) {
			return Result<string>.ForError(StatusCodes.Status400BadRequest, new List<ApplicationError> {
				new ApplicationError("NoPasswordFound", "User does not have a password (OAuth)")
			});
		}

		var passwordValidator = new PasswordValidator<ApplicationUser>();
		var passwordValidateResult = await passwordValidator.ValidateAsync(userManager, null, request.Password);

		if (!passwordValidateResult.Succeeded) {
			return Result<string>.ForError(StatusCodes.Status400BadRequest, new List<ApplicationError> {
				new ApplicationError("InvalidPassword", "The password given is incorrect")
			});
		}

		var result = await signInManager.PasswordSignInAsync(user, request.Password, isPersistent: false, lockoutOnFailure: false);

		if (!result.Succeeded) {
			return Result<string>.ForError(StatusCodes.Status400BadRequest, new List<ApplicationError> {
				new ApplicationError("LoginFailure", "Could not sign in user")
			});
		}

		string token = jwtService.CreateToken(user);
		return Result<string>.ForSuccess(200, token);
	}
}
