using FluentValidation;

namespace Serenity.Application.Identity.Commands.Register.UserDetails;

public class UserDetailsRegisterUserCommandValidator : AbstractValidator<UserDetailsRegisterUserCommand> {
	public UserDetailsRegisterUserCommandValidator() {
		RuleFor(x => x.Email).EmailAddress();
		RuleFor(x => x.AvatarUrl).Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).When(x => !string.IsNullOrEmpty(x.AvatarUrl));
		RuleFor(x => x.Pronouns).Matches(@".+/.+");
		RuleFor(x => x.Gender).Matches(@"\b(Male|Female|Non-Binary|Prefer not to say)\b");
	}
}
