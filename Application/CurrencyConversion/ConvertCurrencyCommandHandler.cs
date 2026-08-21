using Microsoft.EntityFrameworkCore;
using PruebaTecnicaClt.Application.Common;
using PruebaTecnicaClt.Infrastructure.Persistence;

namespace PruebaTecnicaClt.Application.CurrencyConversion;

public sealed class ConvertCurrencyCommandHandler(AppDbContext dbContext)
{
    public async Task<CommandResult<ConversionDto>> HandleAsync(
        ConvertCurrencyCommand command,
        CancellationToken cancellationToken)
    {
        var fromCode = command.FromCode.Trim().ToUpperInvariant();
        var toCode = command.ToCode.Trim().ToUpperInvariant();

        var currencies = await dbContext.Currencies
            .AsNoTracking()
            .Where(currency => currency.Code == fromCode || currency.Code == toCode)
            .ToDictionaryAsync(currency => currency.Code, cancellationToken);

        if (!currencies.TryGetValue(fromCode, out var fromCurrency))
        {
            return CommandResult<ConversionDto>.NotFound($"Divisa '{fromCode}' no encontrada.");
        }

        if (!currencies.TryGetValue(toCode, out var toCurrency))
        {
            return CommandResult<ConversionDto>.NotFound($"Divisa '{toCode}' no encontrada.");
        }

        var baseAmount = command.Amount * fromCurrency.RateToBase;
        var convertedAmount = baseAmount / toCurrency.RateToBase;

        return CommandResult<ConversionDto>.Success(new ConversionDto(
            fromCode,
            toCode,
            command.Amount,
            convertedAmount));
    }
}
