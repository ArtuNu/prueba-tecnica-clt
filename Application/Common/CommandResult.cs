namespace PruebaTecnicaClt.Application.Common;

public enum CommandStatus
{
    Success,
    NotFound,
    Conflict
}

public sealed record CommandResult<T>(CommandStatus Status, T? Value = default, string? Error = null)
{
    public static CommandResult<T> Success(T value) => new(CommandStatus.Success, value);

    public static CommandResult<T> NotFound(string error) => new(CommandStatus.NotFound, default, error);

    public static CommandResult<T> Conflict(string error) => new(CommandStatus.Conflict, default, error);
}
