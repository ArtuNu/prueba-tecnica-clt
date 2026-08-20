using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Users.Commands;

public sealed class DeleteUserCommandHandler(AppDbContext dbContext)
{
    public async Task<bool> HandleAsync(int id, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FindAsync([id], cancellationToken);
        if (user is null)
        {
            return false;
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
