using System.Net;

namespace Core.Config;

public class HttpDownloader : IDownloader
{
    private readonly IHttpClient _client;
    private readonly int _downloadTimeoutInMilliseconds;

    public HttpDownloader(IHttpClient client, int downloadTimeoutInMilliseconds)
    {
        _client = client;
        _downloadTimeoutInMilliseconds = downloadTimeoutInMilliseconds;
    }

    public void Dispose()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task<Result<byte[], Error>> Download(string host, string stage, string environment, string component)
    {
        try
        {
            var url = $"{host}/config?stage={stage}&environment={environment}&component={component}";

            using var responseTask = _client.GetAsync(url);
            var completedTask = Task.WaitAny(responseTask, Task.Delay(_downloadTimeoutInMilliseconds));

            // timed out
            if (completedTask == 1)
                return Result<byte[], Error>.Err(new Error(ErrorKind.DownloadFailure,
                    "timed out downloading configuration"));

            using var response = responseTask.Result;
            if (response.StatusCode != HttpStatusCode.OK)
                return Result<byte[], Error>.Err(new Error(ErrorKind.DownloadFailure,
                    $"failed to download zip file containing configuration with url: {url}"));

            var packageData = await response.Content.ReadAsByteArrayAsync();

            return Result<byte[], Error>.Ok(packageData);
        }
        catch (Exception e)
        {
            return Result<byte[], Error>.Err(new Error(ErrorKind.ExceptionThrown,
                $"an exception '{e.GetType().Name}' has been thrown: {e.Message}: {e.StackTrace}"));
        }
    }
}