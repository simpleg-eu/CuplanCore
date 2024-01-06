using ICSharpCode.SharpZipLib.Zip;

namespace Core.Config;

public class ZipExtractor : Extractor
{
    public Result<Empty, Error> Extract(string sourceFilePath, string targetPath)
    {
        FastZip fastZip = new();
        fastZip.ExtractZip(sourceFilePath, targetPath, FastZip.Overwrite.Always, null, null, null, true);
        
        return Result<Empty, Error>.Ok(new Empty());
    }
}