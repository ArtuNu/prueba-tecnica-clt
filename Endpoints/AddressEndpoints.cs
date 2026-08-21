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
            .WithName("CreateUserAddress")
            .WithSummary("Crear una dirección para un usuario")
            .WithDescription("Crea una nueva dirección vinculada al usuario indicado.")
            .Produces<PruebaTecnicaClt.Application.Addresses.AddressDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .ValidateQueryParameters();
        endpoints.MapGet("/users/{userId:int}/addresses", GetUserAddressesAsync)
            .WithTags("Addresses")
            .WithName("GetUserAddresses")
            .WithSummary("Listar las direcciones de un usuario")
            .WithDescription("Permite filtrar por addressId o por coincidencias parciales en los campos de texto.")
            .Produces<IReadOnlyList<PruebaTecnicaClt.Application.Addresses.AddressDto>>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .ValidateQueryParameters("addressId", "street", "city", "country", "zipCode");
        endpoints.MapGet("/addresses", GetAddressesAsync)
            .WithTags("Addresses")
            .WithName("GetAddresses")
            .WithSummary("Listar todas las direcciones")
            .Produces<IReadOnlyList<PruebaTecnicaClt.Application.Addresses.AddressDto>>()
            .ValidateQueryParameters();
        endpoints.MapGet("/addresses/{id:int}", GetAddressByIdAsync)
            .WithTags("Addresses")
            .WithName("GetAddressById")
            .WithSummary("Obtener una dirección")
            .Produces<PruebaTecnicaClt.Application.Addresses.AddressDto>()
            .Produces(StatusCodes.Status404NotFound)
            .ValidateQueryParameters();
        endpoints.MapPut("/addresses/{id:int}", UpdateAddressAsync)
            .WithTags("Addresses")
            .WithName("UpdateAddress")
            .WithSummary("Reemplazar una dirección")
            .WithDescription("Requiere street, city y country; zipCode es opcional.")
            .Produces<PruebaTecnicaClt.Application.Addresses.AddressDto>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .ValidateQueryParameters();
        endpoints.MapPatch("/addresses/{id:int}", PatchAddressAsync)
            .WithTags("Addresses")
            .WithName("PatchAddress")
            .WithSummary("Actualizar parcialmente una dirección")
            .WithDescription("Se debe enviar al menos uno de los campos de la dirección.")
            .Produces<PruebaTecnicaClt.Application.Addresses.AddressDto>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .ValidateQueryParameters();
        endpoints.MapDelete("/addresses/{id:int}", DeleteAddressAsync)
            .WithTags("Addresses")
            .WithName("DeleteAddress")
            .WithSummary("Eliminar una dirección")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
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
        int? addressId,
        string? street,
        string? city,
        string? country,
        string? zipCode,
        IValidator<GetUserAddressesQuery> validator,
        GetUserAddressesQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetUserAddressesQuery(userId, addressId, street, city, country, zipCode);
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
