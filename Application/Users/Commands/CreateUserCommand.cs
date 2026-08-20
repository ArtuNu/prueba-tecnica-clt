namespace PruebaTecnicaClt.Application.Users.Commands;

public sealed record CreateUserCommand(string Name, string Email, string Password);
