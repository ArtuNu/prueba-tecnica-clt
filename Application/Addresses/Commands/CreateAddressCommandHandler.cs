using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Application.Common;
using PruebaTecnicaClt.Domain.Entities;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Addresses.Commands;

public sealed class CreateAddressCommandHandler(AppDbContext dbContext)
{
    public async Task<CommandResult<AddressDto>> HandleAsync(
        int userId,
        CreateAddressCommand command,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken))
        {
            return CommandResult<AddressDto>.NotFound("Usuario no encontrado.");
        }

        var address = new Address
        {
            UserId = userId,
            Street = command.Street.Trim(),
            City = command.City.Trim(),
            Country = command.Country.Trim(),
            ZipCode = string.IsNullOrWhiteSpace(command.ZipCode) ? null : command.ZipCode.Trim()
        };

        dbContext.Addresses.Add(address);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResult<AddressDto>.Success(AddressDto.FromEntity(address));
    }
}
