using FluentValidation;

namespace PruebaTecnicaClt.Application.Users.Commands;

public sealed class PatchUserCommandValidator : AbstractValidator<PatchUserCommand>
{
    public PatchUserCommandValidator()
    {
        RuleFor(command => command)
            .Must(command => command.Name is not null
                || command.Email is not null
                || command.IsActive.HasValue
                || command.Password is not null)
            .WithMessage("Se debe proporcionar al menos un campo.");

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(150)
            .When(command => command.Name is not null);

        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(254)
            .When(command => command.Email is not null);

        RuleFor(command => command.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(100)
            .When(command => command.Password is not null);
    }
}
