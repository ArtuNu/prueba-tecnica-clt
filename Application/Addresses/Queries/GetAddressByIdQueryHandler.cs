using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Addresses.Queries;

public sealed class GetAddressByIdQueryHandler(AppDbContext dbContext)
{
    public Task<AddressDto?> HandleAsync(int id, CancellationToken cancellationToken) =>
        dbContext.Addresses
            .AsNoTracking()
            .Where(address => address.Id == id)
            .Select(address => new AddressDto(
                address.Id,
                address.UserId,
                address.Street,
                address.City,
                address.Country,
                address.ZipCode))
            .SingleOrDefaultAsync(cancellationToken);
}
