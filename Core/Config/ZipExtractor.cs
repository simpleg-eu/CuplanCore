using ICSharpCode.SharpZipLib.Zip;

namespace Core.Config;

public class ZipExtractor : IExtractor
{
    public Result<Empty, Error> Extract(byte[] packageData, string targetPath)
    {
        using MemoryStream stream = new(packageData);
        FastZip fastZip = new();
        fastZip.ExtractZip(stream, targetPath, FastZip.Overwrite.Always, null, null, null, true, false);
        
        return Result<Empty, Error>.Ok(new Empty());
    }
}