namespace Core.Config;

public interface IConfigProvider : IDisposable
{
    /// <summary>
    ///     Tries to get a configuration by its key.
    /// </summary>
    /// <param name="filePath">
    ///     File from which the configuration will be extracted.
    /// </param>
    /// <param name="key">
    ///     Key of the configuration, levels being separated by an ':'.
    /// </param>
    /// <returns>
    ///     A result containing the value of the specified configuration key
    ///     or an <see cref="Error" /> if the configuration could not be retrieved or it doesn't exist.
    /// </returns>
    /// <throws><see cref="InvalidOperationException" /> if the configuration located at the specified key is not of type T.</throws>
    public Task<Result<T, Error>> Get<T>(string filePath, string key);

    /// <summary>
    /// Cleans any cache mechanism.
    /// </summary>
    public void CleanCache();
}