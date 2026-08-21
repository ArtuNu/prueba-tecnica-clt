using FluentValidation;

namespace PruebaTecnicaClt.Application.CurrencyConversion;

public sealed class ConvertCurrencyCommandValidator : AbstractValidator<ConvertCurrencyCommand>
{
    public ConvertCurrencyCommandValidator()
    {
        RuleFor(command => command.FromCode)
            .NotEmpty()
            .Matches("^[A-Za-z]{3}$")
            .WithMessage("FromCode debe contener exactamente tres letras.");

        RuleFor(command => command.ToCode)
            .NotEmpty()
            .Matches("^[A-Za-z]{3}$")
            .WithMessage("ToCode debe contener exactamente tres letras.");

        RuleFor(command => command.Amount)
            .GreaterThan(0);
    }
}
