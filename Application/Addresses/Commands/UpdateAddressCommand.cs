namespace PruebaTecnicaClt.Application.Addresses.Commands;

public sealed record UpdateAddressCommand(
    string Street,
    string City,
    string Country,
    string? ZipCode) : IAddressFields;
