namespace Core;

public class Error<TErrorKind>(TErrorKind errorKind, string message) where TErrorKind : Enum
{
    public TErrorKind ErrorKind { get; } = errorKind;
    public string Message { get; } = message;
}