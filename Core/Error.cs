namespace Core;

public class Error<TErrorKind>(TErrorKind errorKind, string message)
{
    public TErrorKind ErrorKind { get; } = errorKind;
    public string Message { get; } = message;

    public override bool Equals(object? obj)
    {
        Error<TErrorKind>? other = obj as Error<TErrorKind>;
        if (other is null) return false;

        return ErrorKind != null && ErrorKind.Equals(other.ErrorKind) && Message.Equals(other.Message);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ErrorKind, Message);
    }
}