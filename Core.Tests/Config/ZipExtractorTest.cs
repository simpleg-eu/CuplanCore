using Core;
using Core.Config;
using Xunit;

namespace Cuplan.Core.Tests.Config;

public class ZipExtractorTest : TestBase
{
    [Fact]
    public void Extract_DummyZip_ExtractsCorrectly()
    {
        Extractor extractor = new ZipExtractor();
        string guid = Guid.NewGuid().ToString();
        byte[] packageData = File.ReadAllBytes($"{TestDataPath}/dummy.zip");
        
        extractor.Extract( packageData, guid);
        
        bool expectedDirectoryExists = Directory.Exists($"{guid}");
        bool expectedExecutableWithinDirectoryExists = File.Exists($"{guid}/cp-config");
        bool expectedConfigFileWithinDirectoryExists = File.Exists($"{guid}/config/config.yaml");
        bool expectedLogConfigFileWithinDirectoryExists = File.Exists($"{guid}/config/log4rs.yaml");
        Directory.Delete(guid, true);
        Assert.True(expectedDirectoryExists, "Expected directory does not exist.");
        Assert.True(expectedExecutableWithinDirectoryExists, "Expected executable file does not exist.");
        Assert.True(expectedConfigFileWithinDirectoryExists, "Expected configuration file does not exist.");
        Assert.True(expectedLogConfigFileWithinDirectoryExists, "Expected log configuration file does not exist.");
    }
}