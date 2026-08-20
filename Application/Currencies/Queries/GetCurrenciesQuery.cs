namespace PruebaTecnicaClt.Application.Currencies.Queries;

public sealed record GetCurrenciesQuery(
    string? Code,
    string? Name);
