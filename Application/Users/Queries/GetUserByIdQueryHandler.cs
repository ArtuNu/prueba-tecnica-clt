using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Users.Queries;

public sealed class GetUserByIdQueryHandler(AppDbContext dbContext)
{
    public Task<UserDto?> HandleAsync(
        GetUserByIdQuery query,
        CancellationToken cancellationToken) =>
        dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == query.Id)
            .Select(user => new UserDto(user.Id, user.Name, user.Email, user.IsActive))
            .SingleOrDefaultAsync(cancellationToken);
}
