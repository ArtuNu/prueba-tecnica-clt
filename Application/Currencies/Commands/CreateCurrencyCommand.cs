namespace PruebaTecnicaClt.Application.Currencies.Commands;

public sealed record CreateCurrencyCommand(string Code, string Name, decimal RateToBase);
