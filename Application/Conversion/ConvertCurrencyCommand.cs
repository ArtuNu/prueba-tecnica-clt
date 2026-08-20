namespace PruebaTecnicaClt.Application.Conversion;

public sealed record ConvertCurrencyCommand(string FromCode, string ToCode, decimal Amount);
