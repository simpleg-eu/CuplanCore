namespace Core.Config;

public class DefaultClientFactory : IClientFactory
{
    public Result<Client, Error> Build(string accessToken, string host, string stage, string environment,
        string component,
        string workingPath,
        int downloadTimeoutInMilliseconds)
    {
        HttpClientWrapper wrapper = new();
        wrapper.AuthorizationBearerToken = accessToken;

        IDownloader downloader = new HttpDownloader(wrapper, downloadTimeoutInMilliseconds);
        IExtractor extractor = new ZipExtractor();
        IGetter getter = new FileGetter(workingPath, new Cache(TimeSpan.FromHours(1)), TimeSpan.FromHours(1));

        return Result<Client, Error>.Ok(new Client(host, stage, environment, component, workingPath, downloader,
            extractor,
            getter));
    }
}