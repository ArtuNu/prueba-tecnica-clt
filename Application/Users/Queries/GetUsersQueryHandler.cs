using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Users.Queries;

public sealed class GetUsersQueryHandler(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<UserDto>> HandleAsync(
        GetUsersQuery query,
        CancellationToken cancellationToken)
    {
        var usersQuery = dbContext.Users.AsNoTracking();

        if (query.IsActive.HasValue)
        {
            usersQuery = usersQuery.Where(user => user.IsActive == query.IsActive.Value);
        }

        return await usersQuery
            .OrderBy(user => user.Id)
            .Select(user => new UserDto(user.Id, user.Name, user.Email, user.IsActive))
            .ToListAsync(cancellationToken);
    }
}
