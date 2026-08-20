using PruebaTecnicaClt.Domain.Entities;

namespace PruebaTecnicaClt.Application.Addresses;

public sealed record AddressDto(
    int Id,
    int UserId,
    string Street,
    string City,
    string Country,
    string? ZipCode)
{
    public static AddressDto FromEntity(Address address) =>
        new(address.Id, address.UserId, address.Street, address.City, address.Country, address.ZipCode);
}
