using PruebaTecnicaClt.Domain.Entities;

namespace PruebaTecnicaClt.Application.Currencies;

public sealed record CurrencyDto(int Id, string Code, string Name, decimal RateToBase)
{
    public static CurrencyDto FromEntity(Currency currency) =>
        new(currency.Id, currency.Code, currency.Name, currency.RateToBase);
}
