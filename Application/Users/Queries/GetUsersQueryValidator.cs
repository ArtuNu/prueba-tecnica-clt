using FluentValidation;

namespace PruebaTecnicaClt.Application.Users.Queries;

public sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(query => query.Name)
            .NotEmpty()
            .MaximumLength(150)
            .When(query => query.Name is not null);

        RuleFor(query => query.Email)
            .NotEmpty()
            .MaximumLength(254)
            .When(query => query.Email is not null);

        RuleFor(query => query.IsActive)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(value => bool.TryParse(value, out _))
            .WithMessage("El valor de 'isActive' debe ser 'true' o 'false'.")
            .When(query => query.IsActive is not null);
    }
}
