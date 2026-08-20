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

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            var name = query.Name.Trim().ToLower();
            usersQuery = usersQuery.Where(user => user.Name.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(query.Email))
        {
            var email = query.Email.Trim().ToLower();
            usersQuery = usersQuery.Where(user => user.Email.ToLower().Contains(email));
        }

        if (!string.IsNullOrWhiteSpace(query.IsActive))
        {
            if (bool.TryParse(query.IsActive, out var isActive))
            {
                usersQuery = usersQuery.Where(user => user.IsActive == isActive);
            }
        }

        return await usersQuery
            .OrderBy(user => user.Id)
            .Select(user => new UserDto(user.Id, user.Name, user.Email, user.IsActive))
            .ToListAsync(cancellationToken);
    }
}
