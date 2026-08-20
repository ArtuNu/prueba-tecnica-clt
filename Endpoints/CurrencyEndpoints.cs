using FluentValidation;
using PruebaTecnicaClt.Application.Common;
using PruebaTecnicaClt.Application.Conversion;
using PruebaTecnicaClt.Application.Currencies.Commands;
using PruebaTecnicaClt.Application.Currencies.Queries;

namespace PruebaTecnicaClt.Endpoints;

public static class CurrencyEndpoints
{
    public static IEndpointRouteBuilder MapCurrencyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/currencies", GetCurrenciesAsync).WithTags("Currencies");
        endpoints.MapPost("/currencies", CreateCurrencyAsync).WithTags("Currencies");
        endpoints.MapPost("/currency/convert", ConvertCurrencyAsync).WithTags("Conversion");

        return endpoints;
    }

    private static async Task<IResult> GetCurrenciesAsync(
        string? code,
        string? name,
        GetCurrenciesQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var currencies = await handler.HandleAsync(
            new GetCurrenciesQuery(code, name),
            cancellationToken);
        return Results.Ok(currencies);
    }

    private static async Task<IResult> CreateCurrencyAsync(
        CreateCurrencyCommand command,
        IValidator<CreateCurrencyCommand> validator,
        CreateCurrencyCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToErrorDictionary());
        }

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.Status == CommandStatus.Conflict
            ? Results.Conflict(new { error = result.Error })
            : Results.Created($"/currencies/{result.Value!.Id}", result.Value);
    }

    private static async Task<IResult> ConvertCurrencyAsync(
        ConvertCurrencyCommand command,
        IValidator<ConvertCurrencyCommand> validator,
        ConvertCurrencyCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToErrorDictionary());
        }

        var result = await handler.HandleAsync(command, cancellationToken);
        return result.Status == CommandStatus.NotFound
            ? Results.NotFound(new { error = result.Error })
            : Results.Ok(result.Value);
    }
}
