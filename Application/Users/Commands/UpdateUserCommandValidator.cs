using FluentValidation;

namespace PruebaTecnicaClt.Application.Users.Commands;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(254);

        When(command => !string.IsNullOrWhiteSpace(command.Password), () =>
        {
            RuleFor(command => command.Password!)
                .MinimumLength(8)
                .MaximumLength(100);
        });
    }
}
