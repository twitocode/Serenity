using FluentValidation;

namespace Serenity.Application.Identity.Commands.Register.DisplayName;

public class DisplayNameRegisterUserCommandValidator : AbstractValidator<DisplayNameRegisterUserCommand> {
	public DisplayNameRegisterUserCommandValidator() {
		RuleFor(x => x.DisplayName).MinimumLength(3);
		RuleFor(x => x.Password).MinimumLength(6);
	}
}
