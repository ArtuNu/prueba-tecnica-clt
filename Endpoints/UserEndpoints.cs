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

        group.MapPost("/", CreateUserAsync);
        group.MapGet("/", GetUsersAsync);
        group.MapGet("/{id:int}", GetUserByIdAsync);
        group.MapPut("/{id:int}", UpdateUserAsync);
        group.MapPatch("/{id:int}", PatchUserAsync);
        group.MapDelete("/{id:int}", DeleteUserAsync);

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
        HttpRequest request,
        GetUsersQueryHandler handler,
        CancellationToken cancellationToken)
    {
        // Se valida que los parámetros no se envien vacios

        if (request.Query.ContainsKey("name") &&
            string.IsNullOrWhiteSpace(request.Query["name"]))
        {
            return Results.BadRequest(new
            {
                error = "El parámetro 'name' no puede estar vacío."
            });
        }

        if (request.Query.ContainsKey("email") &&
            string.IsNullOrWhiteSpace(request.Query["email"]))
        {
            return Results.BadRequest(new
            {
                error = "El parámetro 'email' no puede estar vacío."
            });
        }

        if (request.Query.ContainsKey("isActive") &&
            string.IsNullOrWhiteSpace(request.Query["isActive"]))
        {
            return Results.BadRequest(new
            {
                error = "El parámetro 'isActive' no puede estar vacío."
            });
        }

        // Se valida que el valor del parametro sea solo o true o false
        if (!string.IsNullOrWhiteSpace(isActive) &&
            !bool.TryParse(isActive, out _))
        {
            return Results.BadRequest(new
            {
                error = "El parámetro 'isActive' debe ser 'true' o 'false'."
            });
        }

        var users = await handler.HandleAsync(
            new GetUsersQuery(name, email, isActive),
            cancellationToken);
        return Results.Ok(users);
    }

    private static async Task<IResult> GetUserByIdAsync(
        int id,
        GetUserByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var user = await handler.HandleAsync(id, cancellationToken);
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
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}
