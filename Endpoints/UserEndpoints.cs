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

        group.MapPost("/", CreateUserAsync).ValidateQueryParameters();
        group.MapGet("/", GetUsersAsync).ValidateQueryParameters("name", "email", "isActive");
        group.MapGet("/{id:int}", GetUserByIdAsync).ValidateQueryParameters();
        group.MapPut("/{id:int}", UpdateUserAsync).ValidateQueryParameters();
        group.MapPatch("/{id:int}", PatchUserAsync).ValidateQueryParameters();
        group.MapDelete("/{id:int}", DeleteUserAsync).ValidateQueryParameters();

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
        return deleted ? Results.NoContent() : Results.NotFound(new {error = $"No se encontró el usuario con ID {id}."});
    }
}
