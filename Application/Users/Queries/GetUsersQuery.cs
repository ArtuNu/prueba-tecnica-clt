namespace PruebaTecnicaClt.Application.Users.Queries;

public sealed record GetUsersQuery(
    string? Name,
    string? Email,
    string? IsActive);
