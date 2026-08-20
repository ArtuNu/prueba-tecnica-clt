using FluentValidation;

namespace PruebaTecnicaClt.Application.Currencies.Queries;

public sealed class GetCurrenciesQueryValidator : AbstractValidator<GetCurrenciesQuery>
{
    public GetCurrenciesQueryValidator()
    {
        RuleFor(query => query.Code)
            .NotEmpty()
            .MaximumLength(3)
            .When(query => query.Code is not null);

        RuleFor(query => query.Name)
            .NotEmpty()
            .MaximumLength(100)
            .When(query => query.Name is not null);
    }
}
