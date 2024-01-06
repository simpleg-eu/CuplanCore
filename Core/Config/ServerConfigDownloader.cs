using System.Net;

namespace Core.Config;

public class ServerConfigDownloader : IConfigDownloader
{
    private readonly IHttpClient _client;

    public ServerConfigDownloader(IHttpClient client)
    {
        _client = client;
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public async Task<Result<byte[], Error>> Download(string host, string stage, string environment, string component)
    {
        try
        {
            string url = $"{host}/config?stage={stage}&environment={environment}&component={component}";
            
            using HttpResponseMessage response = await _client.GetAsync(url);

            if (response.StatusCode != HttpStatusCode.OK)
                return Result<byte[], Error>.Err(new Error(ErrorKind.DownloadFailure,
                    $"failed to download zip file containing configuration with url: {url}"));

            byte[] packageData = await response.Content.ReadAsByteArrayAsync();

            return Result<byte[], Error>.Ok(packageData);
        }
        catch (Exception e)
        {
            return Result<byte[], Error>.Err(new Error(ErrorKind.ExceptionThrown,
                $"an exception '{e.GetType().Name}' has been thrown: {e.Message}: {e.StackTrace}"));
        }
    }
}