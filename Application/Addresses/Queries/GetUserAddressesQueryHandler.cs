using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Addresses.Queries;

public sealed class GetUserAddressesQueryHandler(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<AddressDto>?> HandleAsync(
        GetUserAddressesQuery query,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Users.AnyAsync(user => user.Id == query.UserId, cancellationToken))
        {
            return null;
        }

        var addressesQuery = dbContext.Addresses
            .AsNoTracking()
            .Where(address => address.UserId == query.UserId);

        if (query.Id.HasValue)
        {
            addressesQuery = addressesQuery.Where(address => address.Id == query.Id.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Street))
        {
            var street = query.Street.Trim().ToLower();
            addressesQuery = addressesQuery.Where(address => address.Street.ToLower().Contains(street));
        }

        if (!string.IsNullOrWhiteSpace(query.City))
        {
            var city = query.City.Trim().ToLower();
            addressesQuery = addressesQuery.Where(address => address.City.ToLower().Contains(city));
        }

        if (!string.IsNullOrWhiteSpace(query.Country))
        {
            var country = query.Country.Trim().ToLower();
            addressesQuery = addressesQuery.Where(address => address.Country.ToLower().Contains(country));
        }

        if (!string.IsNullOrWhiteSpace(query.ZipCode))
        {
            var zipCode = query.ZipCode.Trim().ToLower();
            addressesQuery = addressesQuery.Where(address =>
                address.ZipCode != null && address.ZipCode.ToLower().Contains(zipCode));
        }

        return await addressesQuery
            .OrderBy(address => address.Id)
            .Select(address => new AddressDto(
                address.Id,
                address.UserId,
                address.Street,
                address.City,
                address.Country,
                address.ZipCode))
            .ToListAsync(cancellationToken);
    }
}
