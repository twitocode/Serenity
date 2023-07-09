using FluentValidation;
using Serenity.Application.Identity.Queries.OAuth;

namespace Serenity.Application.Identity.Commands.Register.OAuth;

public class OAuthRegisterUserCommandValidator : AbstractValidator<OAuthRegisterUserCommand> {
	public OAuthRegisterUserCommandValidator() {
		RuleFor(x => x.DisplayName).MinimumLength(3);
	}
}
