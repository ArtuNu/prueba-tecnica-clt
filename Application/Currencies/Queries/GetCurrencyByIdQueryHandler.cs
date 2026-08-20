using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Currencies.Queries;

public sealed class GetCurrencyByIdQueryHandler(AppDbContext dbContext)
{
    public Task<CurrencyDto?> HandleAsync(int id, CancellationToken cancellationToken) =>
        dbContext.Currencies
            .AsNoTracking()
            .Where(currency => currency.Id == id)
            .Select(currency => new CurrencyDto(
                currency.Id,
                currency.Code,
                currency.Name,
                currency.RateToBase))
            .SingleOrDefaultAsync(cancellationToken);
}
