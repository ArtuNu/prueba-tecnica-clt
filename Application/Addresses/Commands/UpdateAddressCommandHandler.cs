using PruebaTecnicaClt.Application.Common;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Addresses.Commands;

public sealed class UpdateAddressCommandHandler(AppDbContext dbContext)
{
    public async Task<CommandResult<AddressDto>> HandleAsync(
        int id,
        UpdateAddressCommand command,
        CancellationToken cancellationToken)
    {
        var address = await dbContext.Addresses.FindAsync([id], cancellationToken);
        if (address is null)
        {
            return CommandResult<AddressDto>.NotFound("Address not found.");
        }

        address.Street = command.Street.Trim();
        address.City = command.City.Trim();
        address.Country = command.Country.Trim();
        address.ZipCode = string.IsNullOrWhiteSpace(command.ZipCode) ? null : command.ZipCode.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);
        return CommandResult<AddressDto>.Success(AddressDto.FromEntity(address));
    }
}
