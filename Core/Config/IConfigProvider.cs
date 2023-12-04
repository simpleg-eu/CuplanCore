namespace Core.Config;

public interface IConfigProvider : IDisposable
{
    /// <summary>
    ///     Tries to get a configuration by its key.
    /// </summary>
    /// <param name="key">
    ///     Key which is going to be searched. It can contain multiple levels which must be separated by ':'.
    ///     Also, the file can be indicated by using the following format: 'directory/file'.
    ///     Finally, an example of a key: 'directory/file|key:sub-key'.
    /// </param>
    /// <returns>
    ///     A result containing the value of the specified configuration key
    ///     or an <see cref="Error{TErrorKind}" /> if the configuration could not be retrieved or it doesn't exist.
    /// </returns>
    /// <throws><see cref="InvalidOperationException" /> if the configuration located at the specified key is not of type T.</throws>
    public Task<Result<T, Error<string>>> Get<T>(string key);
}