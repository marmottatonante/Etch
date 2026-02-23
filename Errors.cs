namespace Etch;

public enum ErrorCode : byte
{
    Clipping = 1,
}

public readonly struct Error(ErrorCode code, string? message)
{
    public ErrorCode Code { get; } = code;
    public string? Message { get; } = message;
}

public static class Errors
{
    private static readonly List<Error> _errors = [];
    public static void Clipping(string? message = null) => _errors.Add(new(ErrorCode.Clipping, message));
}