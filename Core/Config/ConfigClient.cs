namespace Core.Config;

public class ConfigClient : IDisposable
{
    private readonly string _host;
    private readonly string _stage;
    private readonly string _environment;
    private readonly string _component;
    private readonly IConfigDownloader _downloader;
    private readonly IExtractor _extractor;
    private readonly IConfigProvider _provider;
    
    public ConfigClient(string host, string stage, string environment, string component, IConfigDownloader downloader,
        IExtractor extractor, IConfigProvider provider)
    {
        _host = host;
        _stage = stage;
        _environment = environment;
        _component = component;
        _downloader = downloader;
        _extractor = extractor;
        _provider = provider;
    }

    public void Dispose()
    {
    }

    public async Task<Result<T, Error>> Get<T>(string filePath, string key)
    {
        return await _provider.Get<T>(filePath, key);
    }
}