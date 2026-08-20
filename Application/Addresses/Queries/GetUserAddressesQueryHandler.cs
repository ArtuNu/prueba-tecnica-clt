using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Addresses.Queries;

public sealed class GetUserAddressesQueryHandler(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<AddressDto>?> HandleAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken))
        {
            return null;
        }

        return await dbContext.Addresses
            .AsNoTracking()
            .Where(address => address.UserId == userId)
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
