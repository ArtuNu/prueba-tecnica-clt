namespace PruebaTecnicaClt.Application.Addresses.Queries;

public sealed record GetUserAddressesQuery(
    int UserId,
    int? Id,
    string? Street,
    string? City,
    string? Country,
    string? ZipCode);
