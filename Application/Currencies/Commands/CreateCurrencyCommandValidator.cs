using FluentValidation;

namespace PruebaTecnicaClt.Application.Currencies.Commands;

public sealed class CreateCurrencyCommandValidator : AbstractValidator<CreateCurrencyCommand>
{
    public CreateCurrencyCommandValidator()
    {
        RuleFor(command => command.Code)
            .NotEmpty()
            .Matches("^[A-Za-z]{3}$")
            .WithMessage("Code must contain exactly three letters.");

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.RateToBase)
            .GreaterThan(0);
    }
}
