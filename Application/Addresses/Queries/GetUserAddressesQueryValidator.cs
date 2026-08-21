using FluentValidation;

namespace PruebaTecnicaClt.Application.Addresses.Queries;

public sealed class GetUserAddressesQueryValidator : AbstractValidator<GetUserAddressesQuery>
{
    public GetUserAddressesQueryValidator()
    {
        RuleFor(query => query.UserId)
            .GreaterThan(0);

        RuleFor(query => query.AddressId)
            .GreaterThan(0)
            .When(query => query.AddressId.HasValue);

        RuleFor(query => query.Street)
            .NotEmpty()
            .MaximumLength(200)
            .When(query => query.Street is not null);

        RuleFor(query => query.City)
            .NotEmpty()
            .MaximumLength(100)
            .When(query => query.City is not null);

        RuleFor(query => query.Country)
            .NotEmpty()
            .MaximumLength(100)
            .When(query => query.Country is not null);

        RuleFor(query => query.ZipCode)
            .NotEmpty()
            .MaximumLength(20)
            .When(query => query.ZipCode is not null);
    }
}
