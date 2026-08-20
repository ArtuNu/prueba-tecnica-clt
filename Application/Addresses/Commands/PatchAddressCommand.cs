namespace PruebaTecnicaClt.Application.Addresses.Commands;

public sealed record PatchAddressCommand(
    string? Street,
    string? City,
    string? Country,
    string? ZipCode);
