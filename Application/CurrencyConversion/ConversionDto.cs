namespace PruebaTecnicaClt.Application.CurrencyConversion;

public sealed record ConversionDto(
    string FromCode,
    string ToCode,
    decimal Amount,
    decimal ConvertedAmount);
