using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.Currencies.Queries;

public sealed class GetCurrenciesQueryHandler(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<CurrencyDto>> HandleAsync(
        GetCurrenciesQuery query,
        CancellationToken cancellationToken)
    {
        var currenciesQuery = dbContext.Currencies.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            var code = query.Code.Trim().ToLower();
            currenciesQuery = currenciesQuery.Where(currency => currency.Code.ToLower().Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            var name = query.Name.Trim().ToLower();
            currenciesQuery = currenciesQuery.Where(currency => currency.Name.ToLower().Contains(name));
        }

        return await currenciesQuery
            .OrderBy(currency => currency.Code)
            .Select(currency => new CurrencyDto(
                currency.Id,
                currency.Code,
                currency.Name,
                currency.RateToBase))
            .ToListAsync(cancellationToken);
    }
}
