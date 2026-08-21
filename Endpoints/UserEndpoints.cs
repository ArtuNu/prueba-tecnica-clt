using FluentValidation;
using PruebaTecnicaClt.Application.Common;
using PruebaTecnicaClt.Application.Users;
using PruebaTecnicaClt.Application.Users.Commands;
using PruebaTecnicaClt.Application.Users.Queries;

namespace PruebaTecnicaClt.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/users").WithTags("Users");

        group.MapPost("/", CreateUserAsync)
            .WithName("CreateUser")
            .WithSummary("Crear un usuario")
            .WithDescription("Crea un usuario activo. El email debe ser único y el password se almacena como hash.")
            .Produces<UserDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status409Conflict)
            .ValidateQueryParameters();
        group.MapGet("/", GetUsersAsync)
            .WithName("GetUsers")
            .WithSummary("Listar usuarios")
            .WithDescription("Permite filtrar por coincidencia parcial de nombre o email y por estado activo.")
            .Produces<IReadOnlyList<UserDto>>()
            .ProducesValidationProblem()
            .ValidateQueryParameters("name", "email", "isActive");
        group.MapGet("/{id:int}", GetUserByIdAsync)
            .WithName("GetUserById")
            .WithSummary("Obtener un usuario")
            .Produces<UserDto>()
            .Produces(StatusCodes.Status404NotFound)
            .ValidateQueryParameters();
        group.MapPut("/{id:int}", UpdateUserAsync)
            .WithName("UpdateUser")
            .WithSummary("Reemplazar un usuario")
            .WithDescription("Requiere name, email e isActive. Si se omite password, se conserva el actual.")
            .Produces<UserDto>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .ValidateQueryParameters();
        group.MapPatch("/{id:int}", PatchUserAsync)
            .WithName("PatchUser")
            .WithSummary("Actualizar parcialmente un usuario")
            .WithDescription("Acepta name, email, isActive o password; se debe enviar al menos un campo.")
            .Produces<UserDto>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .ValidateQueryParameters();
        group.MapDelete("/{id:int}", DeleteUserAsync)
            .WithName("DeleteUser")
            .WithSummary("Eliminar un usuario")
            .WithDescription("Elimina también todas las direcciones asociadas al usuario.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ValidateQueryParameters();

        return endpoints;
    }

    private static async Task<IResult> CreateUserAsync(
        CreateUserCommand command,
        IValidator<CreateUserCommand> validator,
        CreateUserCommandHandler handler,
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
            : Results.Created($"/users/{result.Value!.Id}", result.Value);
    }

    private static async Task<IResult> GetUsersAsync(
        string? name,
        string? email,
        string? isActive,
        IValidator<GetUsersQuery> validator,
        GetUsersQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetUsersQuery(name, email, isActive);
        var validation = await validator.ValidateAsync(query, cancellationToken);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToErrorDictionary());
        }

        var users = await handler.HandleAsync(query, cancellationToken);
        return Results.Ok(users);
    }

    private static async Task<IResult> GetUserByIdAsync(
        int id,
        GetUserByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var user = await handler.HandleAsync(query, cancellationToken);
        return user is null ? Results.NotFound() : Results.Ok(user);
    }

    private static async Task<IResult> UpdateUserAsync(
        int id,
        UpdateUserCommand command,
        IValidator<UpdateUserCommand> validator,
        UpdateUserCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToErrorDictionary());
        }

        var result = await handler.HandleAsync(id, command, cancellationToken);
        return result.Status switch
        {
            CommandStatus.NotFound => Results.NotFound(new { error = result.Error }),
            CommandStatus.Conflict => Results.Conflict(new { error = result.Error }),
            _ => Results.Ok(result.Value)
        };
    }

    private static async Task<IResult> PatchUserAsync(
        int id,
        PatchUserCommand command,
        IValidator<PatchUserCommand> validator,
        PatchUserCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(command, cancellationToken);
        if (!validation.IsValid)
        {
            return Results.ValidationProblem(validation.ToErrorDictionary());
        }

        var result = await handler.HandleAsync(id, command, cancellationToken);
        return result.Status switch
        {
            CommandStatus.NotFound => Results.NotFound(new { error = result.Error }),
            CommandStatus.Conflict => Results.Conflict(new { error = result.Error }),
            _ => Results.Ok(result.Value)
        };
    }

    private static async Task<IResult> DeleteUserAsync(
        int id,
        DeleteUserCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var deleted = await handler.HandleAsync(id, cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound(new {error = $"No se encontró el usuario con ID {id}."});
    }
}
