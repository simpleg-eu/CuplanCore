namespace Core;

public class Error(string errorKind, string message)
{
    public string ErrorKind { get; } = errorKind;
    public string Message { get; } = message;

    public override bool Equals(object? obj)
    {
        Error? other = obj as Error;
        if (other is null) return false;

        return ErrorKind.Equals(other.ErrorKind) && Message.Equals(other.Message);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ErrorKind, Message);
    }

    public override string ToString()
    {
        return $"{ErrorKind}: {Message}";
    }
}