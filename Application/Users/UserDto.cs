using PruebaTecnicaClt.Domain.Entities;

namespace PruebaTecnicaClt.Application.Users;

public sealed record UserDto(int Id, string Name, string Email, bool IsActive)
{
    public static UserDto FromEntity(User user) =>
        new(user.Id, user.Name, user.Email, user.IsActive);
}
