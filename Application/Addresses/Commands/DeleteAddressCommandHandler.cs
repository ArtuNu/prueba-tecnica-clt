using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Addresses.Commands;

public sealed class DeleteAddressCommandHandler(AppDbContext dbContext)
{
    public async Task<bool> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var address = await dbContext.Addresses.FindAsync([id], cancellationToken);
        if (address is null)
        {
            return false;
        }

        dbContext.Addresses.Remove(address);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
