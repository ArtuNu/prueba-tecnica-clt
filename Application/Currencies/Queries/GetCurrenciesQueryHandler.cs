using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Currencies.Queries;

public sealed class GetCurrenciesQueryHandler(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<CurrencyDto>> HandleAsync(CancellationToken cancellationToken) =>
        await dbContext.Currencies
            .AsNoTracking()
            .OrderBy(currency => currency.Code)
            .Select(currency => new CurrencyDto(
                currency.Id,
                currency.Code,
                currency.Name,
                currency.RateToBase))
            .ToListAsync(cancellationToken);
}
