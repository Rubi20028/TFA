using FluentValidation;
using TFA.Domain.Exceptions;

namespace TFA.Domain.UseCases.SignIn;

internal class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(l => l.Login).Cascade(CascadeMode.Stop)
            .NotEmpty().WithErrorCode(ValidationErrorCode.Empty);
        RuleFor(p => p.Password)
            .NotEmpty().WithErrorCode(ValidationErrorCode.Empty);
    }
}