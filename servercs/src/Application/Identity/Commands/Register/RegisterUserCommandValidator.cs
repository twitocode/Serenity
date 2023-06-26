using FluentValidation;
using Serenity.Application.Identity.Queries.OAuth;

namespace Serenity.Application.Identity.Commands.Register;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand> {
	public RegisterUserCommandValidator() {
		RuleFor(x => x.Email).EmailAddress();
		RuleFor(x => x.DisplayName).MinimumLength(3);
		RuleFor(x => x.Password).MinimumLength(6);
		RuleFor(x => x.AvatarUrl).Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).When(x => !string.IsNullOrEmpty(x.AvatarUrl));
		RuleFor(x => x.Pronouns).Matches("\b(?:[A-Za-z]+)/(?:[A-Za-z]+)\b");
		RuleFor(x => x.Gender).Matches("\b(?:Male|Female|Non-Binary|Prefer not to say)\b");
	}
}
