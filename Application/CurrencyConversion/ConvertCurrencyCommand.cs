namespace PruebaTecnicaClt.Application.CurrencyConversion;

public sealed record ConvertCurrencyCommand(string FromCode, string ToCode, decimal Amount);
