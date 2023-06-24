using FluentValidation;
using Serenity.Application.Identity.Queries.OAuth;

namespace Serenity.Application.Identity.Commands.Register;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.DisplayName).MinimumLength(3);
        RuleFor(x => x.Password).MinimumLength(6);
    }
}