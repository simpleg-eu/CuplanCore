namespace Core.Config;

/// <summary>
/// Interface which provides a facility to extract a configuration package.
/// </summary>
public interface Extractor
{
    /// <summary>
    /// Extracts the configuration package's content into the targetPath.
    /// </summary>
    /// <param name="packageData">Package's raw data.</param>
    /// <param name="targetPath">Path where the configuration will be extracted into.</param>
    /// <returns><see cref="Empty"/> if successful, an <see cref="Error"/> otherwise.</returns>
    public Result<Empty, Error> Extract(byte[] packageData, string targetPath);
}