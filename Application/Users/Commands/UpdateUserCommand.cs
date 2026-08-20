namespace PruebaTecnicaClt.Application.Users.Commands;

public sealed record UpdateUserCommand(
    string Name,
    string Email,
    bool IsActive,
    string? Password);
