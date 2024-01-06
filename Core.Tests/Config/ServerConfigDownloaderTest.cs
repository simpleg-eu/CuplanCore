using System.Net;
using Core;
using Core.Config;
using Moq;
using Xunit;

namespace Cuplan.Core.Tests.Config;

public class ServerConfigDownloaderTest : TestBase, IDisposable
{
    private string? _targetDirectory;

    public void Dispose()
    {
        if (Directory.Exists(_targetDirectory)) Directory.Delete(_targetDirectory, true);
    }

    [Fact]
    public async Task Download_ExpectedFile()
    {
        const string exampleUrl = "https://simpleg.eu";
        const string exampleStage = "dummy";
        const string exampleEnvironment = "dummy";
        const string exampleComponent = "dummy";
        const string mockExampleUrl = $"{exampleUrl}/config?stage={exampleStage}&environment={exampleEnvironment}&component={exampleComponent}";
        byte[] exampleFileBytes = await File.ReadAllBytesAsync($"{TestDataPath}/example.zip");
        HttpContent content = new ByteArrayContent(exampleFileBytes);
        HttpResponseMessage response = new();
        response.StatusCode = HttpStatusCode.OK;
        response.Content = content;
        Mock<IHttpClient> httpClient = new();
        httpClient.Setup(h => h.GetAsync(mockExampleUrl))
            .ReturnsAsync(response);

        ServerConfigDownloader downloader = new(httpClient.Object);

        Result<byte[], Error> result = await downloader.Download(exampleUrl, exampleStage, exampleEnvironment, exampleComponent);

        Assert.True(result.IsOk);
        Assert.Equal(exampleFileBytes, result.Unwrap());
    }
}