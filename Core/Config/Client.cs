namespace Core.Config;

public class Client : IDisposable
{
    private readonly string _component;
    private readonly IDownloader _downloader;
    private readonly string _environment;
    private readonly IExtractor _extractor;
    private readonly IGetter _getter;
    private readonly string _host;
    private readonly string _stage;
    private readonly string _workingPath;

    public Client(
        string host,
        string stage,
        string environment,
        string component,
        string workingPath,
        IDownloader downloader,
        IExtractor extractor,
        IGetter getter)
    {
        _host = host;
        _stage = stage;
        _environment = environment;
        _component = component;
        _workingPath = workingPath;
        _downloader = downloader;
        _extractor = extractor;
        _getter = getter;
    }

    public void Dispose()
    {
        Directory.Delete(_workingPath, true);
    }

    public async Task<Result<T, Error>> Get<T>(string filePath, string key)
    {
        if (!Directory.Exists(_workingPath))
        {
            var initResult = await InitializeConfigurationWithinWorkingPath();

            if (!initResult.IsOk) return Result<T, Error>.Err(initResult.UnwrapErr());
        }

        return await _getter.Get<T>(filePath, key);
    }

    private async Task<Result<Empty, Error>> InitializeConfigurationWithinWorkingPath()
    {
        Directory.CreateDirectory(_workingPath);

        var downloadResult = await _downloader.Download(_host, _stage, _environment, _component);

        if (!downloadResult.IsOk) return Result<Empty, Error>.Err(downloadResult.UnwrapErr());

        _getter.CleanCache();
        return _extractor.Extract(downloadResult.Unwrap(), _workingPath);
    }
}