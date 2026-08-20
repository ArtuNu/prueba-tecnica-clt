using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Application.Common;
using PruebaTecnicaClt.Domain.Entities;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Currencies.Commands;

public sealed class CreateCurrencyCommandHandler(AppDbContext dbContext)
{
    public async Task<CommandResult<CurrencyDto>> HandleAsync(
        CreateCurrencyCommand command,
        CancellationToken cancellationToken)
    {
        var normalizedCode = command.Code.Trim().ToUpperInvariant();

        if (await dbContext.Currencies.AnyAsync(
                currency => currency.Code == normalizedCode,
                cancellationToken))
        {
            return CommandResult<CurrencyDto>.Conflict("A currency with this code already exists.");
        }

        var currency = new Currency
        {
            Code = normalizedCode,
            Name = command.Name.Trim(),
            RateToBase = command.RateToBase
        };

        dbContext.Currencies.Add(currency);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return CommandResult<CurrencyDto>.Conflict("A currency with this code already exists.");
        }

        return CommandResult<CurrencyDto>.Success(CurrencyDto.FromEntity(currency));
    }
}
