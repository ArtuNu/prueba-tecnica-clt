using FluentValidation;
using PruebaTecnicaClt.Application.Addresses.Commands;
using PruebaTecnicaClt.Application.Addresses.Queries;
using PruebaTecnicaClt.Application.Common;

namespace PruebaTecnicaClt.Endpoints;

public static class AddressEndpoints
{
    public static IEndpointRouteBuilder MapAddressEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/users/{userId:int}/addresses", CreateAddressAsync).WithTags("Addresses");
        endpoints.MapGet("/users/{userId:int}/addresses", GetUserAddressesAsync).WithTags("Addresses");
        endpoints.MapPut("/addresses/{id:int}", UpdateAddressAsync).WithTags("Addresses");
        endpoints.MapPatch("/addresses/{id:int}", PatchAddressAsync).WithTags("Addresses");
        endpoints.MapDelete("/addresses/{id:int}", DeleteAddressAsync).WithTags("Addresses");

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
        string? street,
        string? city,
        string? country,
        string? zipCode,
        GetUserAddressesQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var addresses = await handler.HandleAsync(
            new GetUserAddressesQuery(userId, street, city, country, zipCode),
            cancellationToken);
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
