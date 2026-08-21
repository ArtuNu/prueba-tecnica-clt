using FluentValidation;
using PruebaTecnicaClt.Application.Addresses.Commands;
using PruebaTecnicaClt.Application.Addresses.Queries;
using PruebaTecnicaClt.Application.Common;

namespace PruebaTecnicaClt.Endpoints;

public static class AddressEndpoints
{
    public static IEndpointRouteBuilder MapAddressEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/users/{userId:int}/addresses", CreateAddressAsync)
            .WithTags("Addresses")
            .ValidateQueryParameters();
        endpoints.MapGet("/users/{userId:int}/addresses", GetUserAddressesAsync)
            .WithTags("Addresses")
            .ValidateQueryParameters("id", "street", "city", "country", "zipCode");
        endpoints.MapGet("/addresses", GetAddressesAsync)
            .WithTags("Addresses")
            .ValidateQueryParameters();
        endpoints.MapGet("/addresses/{id:int}", GetAddressByIdAsync)
            .WithTags("Addresses")
            .ValidateQueryParameters();
        endpoints.MapPut("/addresses/{id:int}", UpdateAddressAsync)
            .WithTags("Addresses")
            .ValidateQueryParameters();
        endpoints.MapPatch("/addresses/{id:int}", PatchAddressAsync)
            .WithTags("Addresses")
            .ValidateQueryParameters();
        endpoints.MapDelete("/addresses/{id:int}", DeleteAddressAsync)
            .WithTags("Addresses")
            .ValidateQueryParameters();

        return endpoints;
    }

    private static async Task<IResult> CreateAddressAsync(
        int userId,
        CreateAddressCommand command,
        IValidator<CreateAddressCommand> validator,
        CreateAddressCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToErrorDictionary());
        }

        var result = await handler.HandleAsync(userId, command, cancellationToken);
        return result.Status switch
        {
            CommandStatus.NotFound => Results.NotFound(new { error = result.Error }),
            CommandStatus.Conflict => Results.Conflict(new { error = result.Error }),
            _ => Results.Created($"/addresses/{result.Value!.Id}", result.Value)
        };
    }

    private static async Task<IResult> GetUserAddressesAsync(
        int userId,
        int? id,
        string? street,
        string? city,
        string? country,
        string? zipCode,
        IValidator<GetUserAddressesQuery> validator,
        GetUserAddressesQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetUserAddressesQuery(userId, id, street, city, country, zipCode);
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToErrorDictionary());
        }

        var addresses = await handler.HandleAsync(query, cancellationToken);
        return addresses is null
            ? Results.NotFound(new { error = "Usuario no encontrado." })
            : Results.Ok(addresses);
    }

    private static async Task<IResult> UpdateAddressAsync(
        int id,
        UpdateAddressCommand command,
        IValidator<UpdateAddressCommand> validator,
        UpdateAddressCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToErrorDictionary());
        }

        var result = await handler.HandleAsync(id, command, cancellationToken);
        return result.Status == CommandStatus.NotFound
            ? Results.NotFound(new { error = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetAddressByIdAsync(
        int id,
        GetAddressByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAddressByIdQuery(id);
        var address = await handler.HandleAsync(query, cancellationToken);
        return address is null
            ? Results.NotFound(new { error = $"No se encontró la dirección con ID {id}." })
            : Results.Ok(address);
    }

    private static async Task<IResult> GetAddressesAsync(
        GetAddressesQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAddressesQuery();
        var addresses = await handler.HandleAsync(query, cancellationToken);
        return Results.Ok(addresses);
    }

    private static async Task<IResult> PatchAddressAsync(
        int id,
        PatchAddressCommand command,
        IValidator<PatchAddressCommand> validator,
        PatchAddressCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToErrorDictionary());
        }

        var result = await handler.HandleAsync(id, command, cancellationToken);
        return result.Status == CommandStatus.NotFound
            ? Results.NotFound(new { error = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteAddressAsync(
        int id,
        DeleteAddressCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var deleted = await handler.HandleAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}
