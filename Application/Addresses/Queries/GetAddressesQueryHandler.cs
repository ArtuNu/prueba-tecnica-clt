using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Addresses.Queries;

public sealed class GetAddressesQueryHandler(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<AddressDto>> HandleAsync(
        GetAddressesQuery query,
        CancellationToken cancellationToken) =>
        await dbContext.Addresses
            .AsNoTracking()
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
