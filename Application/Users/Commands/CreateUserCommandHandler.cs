using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Application.Common;
using PruebaTecnicaClt.Domain.Entities;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Users.Commands;

public sealed class CreateUserCommandHandler(
    AppDbContext dbContext,
    IPasswordHasher<User> passwordHasher)
{
    public async Task<CommandResult<UserDto>> HandleAsync(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        if (await dbContext.Users.AnyAsync(
                user => user.Email == normalizedEmail,
                cancellationToken))
        {
            return CommandResult<UserDto>.Conflict("A user with this email already exists.");
        }

        var user = new User
        {
            Name = command.Name.Trim(),
            Email = normalizedEmail,
            PasswordHash = string.Empty
        };

        user.PasswordHash = passwordHasher.HashPassword(user, command.Password);

        dbContext.Users.Add(user);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult<UserDto>.Conflict("A user with this email already exists.");
        }

        return CommandResult<UserDto>.Success(UserDto.FromEntity(user));
    }
}
