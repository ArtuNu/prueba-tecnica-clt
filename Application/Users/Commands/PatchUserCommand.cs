namespace PruebaTecnicaClt.Application.Users.Commands;

public sealed record PatchUserCommand(
    string? Name,
    string? Email,
    bool? IsActive,
    string? Password);
