using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Application.Common;
using PruebaTecnicaClt.Domain.Entities;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Users.Commands;

public sealed class PatchUserCommandHandler(
    AppDbContext dbContext,
    IPasswordHasher<User> passwordHasher)
{
    public async Task<CommandResult<UserDto>> HandleAsync(
        int id,
        PatchUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FindAsync([id], cancellationToken);
        if (user is null)
        {
            return CommandResult<UserDto>.NotFound("Usuario no encontrado.");
        }

        if (command.Email is not null)
        {
            var normalizedEmail = command.Email.Trim().ToLowerInvariant();
            var emailIsInUse = await dbContext.Users.AnyAsync(
                otherUser => otherUser.Id != id && otherUser.Email == normalizedEmail,
                cancellationToken);

            if (emailIsInUse)
            {
                return CommandResult<UserDto>.Conflict("Ya existe un usuario con este email.");
            }

            user.Email = normalizedEmail;
        }

        if (command.Name is not null)
        {
            user.Name = command.Name.Trim();
        }

        if (command.IsActive.HasValue)
        {
            user.IsActive = command.IsActive.Value;
        }

        if (command.Password is not null)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, command.Password);
        }

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult<UserDto>.Conflict("Ya existe un usuario con este email.");
        }

        return CommandResult<UserDto>.Success(UserDto.FromEntity(user));
    }
}
