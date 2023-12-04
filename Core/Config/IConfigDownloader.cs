namespace Core.Config;

public interface IConfigDownloader : IDisposable
{
    /// <summary>
    ///     Downloads the configuration from the specified URL and component.
    ///     The caller is responsible for deleting the directory where the configuration files have been downloaded into.
    /// </summary>
    /// <param name="url">Url of the configuration provider.</param>
    /// <param name="component">Component to download.</param>
    /// <returns>
    ///     A string indicating the directory where the configuration files have been downloaded into, or an
    ///     <see cref="Error{TErrorKind}" />.
    /// </returns>
    public Task<Result<string, Error<string>>> Download(string url, string component);
}