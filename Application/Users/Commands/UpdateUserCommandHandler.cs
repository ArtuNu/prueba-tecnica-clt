using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Application.Common;
using PruebaTecnicaClt.Domain.Entities;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Users.Commands;

public sealed class UpdateUserCommandHandler(
    AppDbContext dbContext,
    IPasswordHasher<User> passwordHasher)
{
    public async Task<CommandResult<UserDto>> HandleAsync(
        int id,
        UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FindAsync([id], cancellationToken);
        if (user is null)
        {
            return CommandResult<UserDto>.NotFound("Usuario no encontrado.");
        }

        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var emailIsInUse = await dbContext.Users.AnyAsync(
            otherUser => otherUser.Id != id && otherUser.Email == normalizedEmail,
            cancellationToken);

        if (emailIsInUse)
        {
            return CommandResult<UserDto>.Conflict("Ya existe un usuario con este email.");
        }

        user.Name = command.Name.Trim();
        user.Email = normalizedEmail;
        user.IsActive = command.IsActive;

        if (!string.IsNullOrWhiteSpace(command.Password))
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
