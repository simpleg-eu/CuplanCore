using Core;
using Core.Config;
using Moq;
using Xunit;

namespace Cuplan.Core.Tests.Config;

public class ConfigClientTest : TestBase
{
    [Fact]
    public async Task Get_WorkingPathEmpty_DownloadsConfiguration()
    {
        const string expectedValue = "1234abcd";
        const string host = "https://simpleg.eu";
        const string stage = "dummy";
        const string environment = "development";
        const string component = "dummy";
        const string filePath = "application.yaml";
        const string configKey = "Parent:Child";
        byte[] packageData = [];
        Mock<IConfigDownloader> downloaderMock = new(MockBehavior.Strict);
        downloaderMock.Setup(d => d.Download(
            It.Is<string>(h => h == host),
            It.Is<string>(s => s == stage),
            It.Is<string>(e => e == environment),
            It.Is<string>(c => c == component)))
            .Returns(Task.FromResult(Result<byte[], Error>.Ok(packageData)));
        Mock<IExtractor> extractorMock = new(MockBehavior.Strict);
        extractorMock.Setup(e => e.Extract(It.Is<byte[]>(p => p.Equals(packageData)), It.IsAny<string>()))
            .Returns(Result<Empty, Error>.Ok(new Empty()));
        Mock<IConfigProvider> configProviderMock = new(MockBehavior.Strict);
        configProviderMock.Setup(c =>
                c.Get<string>(It.Is<string>(f => f == filePath), It.Is<string>(k => k == configKey)))
            .ReturnsAsync(Result<string, Error>.Ok(expectedValue));
        ConfigClient client = new(host, stage, environment, component,
            downloaderMock.Object, extractorMock.Object, configProviderMock.Object);

        Result<string, Error> value = await client.Get<string>(filePath, configKey);
        
        client.Dispose();
        
        Assert.True(value.IsOk);
        Assert.Equal(expectedValue, value.Unwrap());
    }
}