using FluentValidation;

namespace PruebaTecnicaClt.Application.Addresses.Commands;

public sealed class PatchAddressCommandValidator : AbstractValidator<PatchAddressCommand>
{
    public PatchAddressCommandValidator()
    {
        RuleFor(command => command)
            .Must(command => command.Street is not null
                || command.City is not null
                || command.Country is not null
                || command.ZipCode is not null)
            .WithMessage("Se debe proporcionar al menos un campo.");

        RuleFor(command => command.Street)
            .NotEmpty()
            .MaximumLength(200)
            .When(command => command.Street is not null);

        RuleFor(command => command.City)
            .NotEmpty()
            .MaximumLength(100)
            .When(command => command.City is not null);

        RuleFor(command => command.Country)
            .NotEmpty()
            .MaximumLength(100)
            .When(command => command.Country is not null);

        RuleFor(command => command.ZipCode)
            .MaximumLength(20)
            .When(command => command.ZipCode is not null);
    }
}
