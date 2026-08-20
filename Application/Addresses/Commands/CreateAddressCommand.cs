namespace PruebaTecnicaClt.Application.Addresses.Commands;

public sealed record CreateAddressCommand(
    string Street,
    string City,
    string Country,
    string? ZipCode) : IAddressFields;
