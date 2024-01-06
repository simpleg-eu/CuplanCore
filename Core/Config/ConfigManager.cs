namespace Core.Config;

public class ConfigManager : IDisposable
{
    private readonly IConfigDownloader _downloader;

    public ConfigManager(IConfigDownloader downloader)
    {
        _downloader = downloader;
    }

    public void Dispose()
    {
        _downloader.Dispose();
    }

    /// <summary>
    ///     Creates an <see cref="IConfigProvider" /> which must be disposed once it is no longer used.
    /// </summary>
    /// <param name="url">Url from which to download the configuration.</param>
    /// <param name="component">Component to be downloaded.</param>
    /// <returns>An <see cref="IConfigProvider" /> or an error.</returns>
    public async Task<Result<IConfigProvider, Error>> Download(string url, string component)
    {
        Result<string, Error> downloadResult = await _downloader.Download(url, component);

        if (!downloadResult.IsOk) return Result<IConfigProvider, Error>.Err(downloadResult.UnwrapErr());

        IConfigProvider configProvider = new FileConfigProvider(downloadResult.Unwrap());

        return Result<IConfigProvider, Error>.Ok(configProvider);
    }
}