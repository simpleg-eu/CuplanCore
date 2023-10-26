namespace Core;

public class Result<TOk, TError>
{
    private readonly TError _error;
    private readonly TOk _ok;

    private Result(TOk ok, TError error, bool success)
    {
        _ok = ok;
        _error = error;
        IsOk = success;
    }

    public bool IsOk { get; }

    public TReturn Match<TReturn>(Func<TOk, TReturn> success, Func<TError, TReturn> failure)
    {
        return IsOk ? success(_ok) : failure(_error);
    }

    public static Result<TOk, TError> Ok(TOk ok)
    {
        return new Result<TOk, TError>(ok, default, true);
    }

    public static Result<TOk, TError> Err(TError error)
    {
        return new Result<TOk, TError>(default, error, false);
    }
}