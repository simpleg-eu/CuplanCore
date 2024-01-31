namespace Core.Config;

public class Client : IDisposable
{
    private readonly string _host;
    private readonly string _stage;
    private readonly string _environment;
    private readonly string _component;
    private readonly string _workingPath;
    private readonly double _downloadAgainAfterSeconds;
    private readonly IDownloader _downloader;
    private readonly IExtractor _extractor;
    private readonly IProvider _provider;
    
    private DateTime _lastDownload = DateTime.UnixEpoch;
    
    public Client(
        string host,
        string stage,
        string environment,
        string component,
        string workingPath,
        double downloadAgainAfterSeconds,
        IDownloader downloader,
        IExtractor extractor,
        IProvider provider)
    {
        _host = host;
        _stage = stage;
        _environment = environment;
        _component = component;
        _workingPath = workingPath;
        _downloadAgainAfterSeconds = downloadAgainAfterSeconds;
        _downloader = downloader;
        _extractor = extractor;
        _provider = provider;
    }

    public void Dispose()
    {
        Directory.Delete(_workingPath, true);
    }

    public async Task<Result<T, Error>> Get<T>(string filePath, string key)
    {
        TimeSpan timeSinceLastDownload = DateTime.Now - _lastDownload;
        
        if (timeSinceLastDownload.TotalSeconds > _downloadAgainAfterSeconds ||
            !Directory.Exists(_workingPath))
        {
            Result<Empty, Error> initResult = await InitializeConfigurationWithinWorkingPath();

            if (!initResult.IsOk)
            {
                return Result<T, Error>.Err(initResult.UnwrapErr());
            }
        }
        
        return await _provider.Get<T>(filePath, key);
    }

    private async Task<Result<Empty, Error>> InitializeConfigurationWithinWorkingPath()
    {
        Directory.CreateDirectory(_workingPath);

        Result<byte[], Error> downloadResult = await _downloader.Download(_host, _stage, _environment, _component);

        if (!downloadResult.IsOk)
        {
            return Result<Empty, Error>.Err(downloadResult.UnwrapErr());
        }

        _lastDownload = DateTime.Now;
        _provider.CleanCache();
        return _extractor.Extract(downloadResult.Unwrap(), _workingPath);
    }
}