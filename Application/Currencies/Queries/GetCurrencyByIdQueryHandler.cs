using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Currencies.Queries;

public sealed class GetCurrencyByIdQueryHandler(AppDbContext dbContext)
{
    public Task<CurrencyDto?> HandleAsync(
        GetCurrencyByIdQuery query,
        CancellationToken cancellationToken) =>
        dbContext.Currencies
            .AsNoTracking()
            .Where(currency => currency.Id == query.Id)
            .Select(currency => new CurrencyDto(
                currency.Id,
                currency.Code,
                currency.Name,
                currency.RateToBase))
            .SingleOrDefaultAsync(cancellationToken);
}
