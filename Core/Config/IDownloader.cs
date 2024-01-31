namespace Core.Config;

/// <summary>
/// Interface which provides facility to download configuration packages.
/// </summary>
public interface IDownloader : IDisposable
{
    /// <summary>
    /// Downloads the latest configuration from a specific configuration provider.
    /// </summary>
    /// <param name="host">Host which the configuration package is being downloaded from.</param>
    /// <param name="stage">Flavour of the configuration package to be downloaded.</param>
    /// <param name="environment">Environment from which the configuration package is being downloaded from.</param>
    /// <param name="component">Configuration entity to be downloaded.</param>
    /// <returns>An array of bytes containing the configuration package, or an <see cref="Error"/> if there was a failure.</returns>
    public Task<Result<byte[], Error>> Download(string host, string stage, string environment, string component);
}