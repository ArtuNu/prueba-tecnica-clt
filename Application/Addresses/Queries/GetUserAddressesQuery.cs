namespace PruebaTecnicaClt.Application.Addresses.Queries;

public sealed record GetUserAddressesQuery(
    int UserId,
    int? AddressId,
    string? Street,
    string? City,
    string? Country,
    string? ZipCode);
