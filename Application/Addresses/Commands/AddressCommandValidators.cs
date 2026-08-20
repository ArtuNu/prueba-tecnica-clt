using FluentValidation;

namespace PruebaTecnicaClt.Application.Addresses.Commands;

public sealed class CreateAddressCommandValidator : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressCommandValidator()
    {
        Include(new AddressFieldsValidator());
    }
}

public sealed class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        Include(new AddressFieldsValidator());
    }
}

public interface IAddressFields
{
    string Street { get; }
    string City { get; }
    string Country { get; }
    string? ZipCode { get; }
}

internal sealed class AddressFieldsValidator : AbstractValidator<IAddressFields>
{
    public AddressFieldsValidator()
    {
        RuleFor(command => command.Street).NotEmpty().MaximumLength(200);
        RuleFor(command => command.City).NotEmpty().MaximumLength(100);
        RuleFor(command => command.Country).NotEmpty().MaximumLength(100);
        RuleFor(command => command.ZipCode).MaximumLength(20);
    }
}
