using FluentValidation;
using Serenity.Application.Identity.Queries.OAuth;

namespace Serenity.Application.Identity.Commands.Register.Email;

public class EmailRegisterUserCommandValidator : AbstractValidator<EmailRegisterUserCommand> {
	public EmailRegisterUserCommandValidator() {
		RuleFor(x => x.Email).EmailAddress();
	}
}
