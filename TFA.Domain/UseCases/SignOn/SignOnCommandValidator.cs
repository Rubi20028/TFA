using FluentValidation;
using TFA.Domain.Exceptions;
using TFA.Domain.UseCases.SignIn;

namespace TFA.Domain.UseCases.SignOn;

internal class SignOnCommandValidator : AbstractValidator<SignOnCommand>
{
    public SignOnCommandValidator()
    {
        RuleFor(l => l.Login)
            .NotEmpty().WithErrorCode(ValidationErrorCode.Empty)
            .MaximumLength(20).WithErrorCode(ValidationErrorCode.TooLong);
        RuleFor(p => p.Password)
            .NotEmpty().WithErrorCode(ValidationErrorCode.Empty);
    }
}