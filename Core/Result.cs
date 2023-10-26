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

    public TOk Unwrap()
    {
        if (!IsOk) throw new InvalidOperationException("tried to unwrap 'Ok' when the result was an 'Err'.");

        return _ok;
    }

    public TError UnwrapErr()
    {
        if (IsOk) throw new InvalidOperationException("tired to unwrap 'Err' when the result was an 'Ok'.");

        return _error;
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