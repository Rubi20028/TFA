﻿using FluentValidation;
using TFA.Domain.Exceptions;

namespace TFA.Domain.UseCases.GetTopics;

internal class GetTopicsQueryValidator : AbstractValidator<GetTopicsQuery>
{
    public GetTopicsQueryValidator()
    {
        RuleFor(q => q.ForumId).NotEmpty().WithErrorCode(ValidationErrorCode.Empty);
        RuleFor(q => q.Skip).GreaterThanOrEqualTo(0).WithErrorCode(ValidationErrorCode.invalid);
        RuleFor(q => q.Take).GreaterThanOrEqualTo(0).WithErrorCode(ValidationErrorCode.invalid);
    }
}