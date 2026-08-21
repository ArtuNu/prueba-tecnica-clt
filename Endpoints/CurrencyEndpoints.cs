using FluentValidation;
using PruebaTecnicaClt.Application.Common;
using PruebaTecnicaClt.Application.CurrencyConversion;
using PruebaTecnicaClt.Application.Currencies.Commands;
using PruebaTecnicaClt.Application.Currencies.Queries;

namespace PruebaTecnicaClt.Endpoints;

public static class CurrencyEndpoints
{
    public static IEndpointRouteBuilder MapCurrencyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/currencies", GetCurrenciesAsync)
            .WithTags("Currencies")
            .WithName("GetCurrencies")
            .WithSummary("Listar monedas")
            .WithDescription("Permite filtrar por coincidencia parcial de código o nombre.")
            .Produces<IReadOnlyList<PruebaTecnicaClt.Application.Currencies.CurrencyDto>>()
            .ProducesValidationProblem()
            .ValidateQueryParameters("code", "name");
        endpoints.MapGet("/currencies/{id:int}", GetCurrencyByIdAsync)
            .WithTags("Currencies")
            .WithName("GetCurrencyById")
            .WithSummary("Obtener una moneda")
            .Produces<PruebaTecnicaClt.Application.Currencies.CurrencyDto>()
            .Produces(StatusCodes.Status404NotFound)
            .ValidateQueryParameters();
        endpoints.MapPost("/currencies", CreateCurrencyAsync)
            .WithTags("Currencies")
            .WithName("CreateCurrency")
            .WithSummary("Crear una moneda")
            .WithDescription("El código debe tener tres letras y ser único. RateToBase debe ser mayor que cero.")
            .Produces<PruebaTecnicaClt.Application.Currencies.CurrencyDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status409Conflict)
            .ValidateQueryParameters();
        endpoints.MapPost("/currency/convert", ConvertCurrencyAsync)
            .WithTags("Conversion")
            .WithName("ConvertCurrency")
            .WithSummary("Convertir un importe")
            .WithDescription("Calcula amount × from.rateToBase ÷ to.rateToBase. Ambas monedas deben existir.")
            .Produces<ConversionDto>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .ValidateQueryParameters();

        return endpoints;
    }

    private static async Task<IResult> GetCurrenciesAsync(
        string? code,
        string? name,
        IValidator<GetCurrenciesQuery> validator,
        GetCurrenciesQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetCurrenciesQuery(code, name);
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToErrorDictionary());
        }

        var currencies = await handler.HandleAsync(query, cancellationToken);
        return Results.Ok(currencies);
    }

    private static async Task<IResult> GetCurrencyByIdAsync(
        int id,
        GetCurrencyByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetCurrencyByIdQuery(id);
        var currency = await handler.HandleAsync(query, cancellationToken);
        return currency is null
            ? Results.NotFound(new { error = $"No se encontró la moneda con ID {id}." })
            : Results.Ok(currency);
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
