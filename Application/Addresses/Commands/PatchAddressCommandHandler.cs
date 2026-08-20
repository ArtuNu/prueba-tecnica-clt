using PruebaTecnicaClt.Application.Common;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Addresses.Commands;

public sealed class PatchAddressCommandHandler(AppDbContext dbContext)
{
    public async Task<CommandResult<AddressDto>> HandleAsync(
        int id,
        PatchAddressCommand command,
        CancellationToken cancellationToken)
    {
        var address = await dbContext.Addresses.FindAsync([id], cancellationToken);
        if (address is null)
        {
            return CommandResult<AddressDto>.NotFound("Dirección no encontrada.");
        }

        if (command.Street is not null)
        {
            address.Street = command.Street.Trim();
        }

        if (command.City is not null)
        {
            address.City = command.City.Trim();
        }

        if (command.Country is not null)
        {
            address.Country = command.Country.Trim();
        }

        if (command.ZipCode is not null)
        {
            address.ZipCode = string.IsNullOrWhiteSpace(command.ZipCode)
                ? null
                : command.ZipCode.Trim();
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return CommandResult<AddressDto>.Success(AddressDto.FromEntity(address));
    }
}
