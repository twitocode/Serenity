using FluentValidation;

namespace Serenity.Application.Identity.Commands.Login.Local;

public class LocalLoginCommandValidator : AbstractValidator<LocalLoginCommand> {
	public LocalLoginCommandValidator() {
		RuleFor(x => x.Identifier)
			.NotEmpty();

		RuleFor(x => x.Password)
			.NotEmpty();
	}
}
