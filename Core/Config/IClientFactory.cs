namespace Core.Config;

public interface IClientFactory
{
    /// <summary>
    ///     Builds a <see cref="Client" /> instance.
    /// </summary>
    /// <param name="host">The URL of the configuration server.</param>
    /// <param name="stage">Stage of the configuration to be retrieved.</param>
    /// <param name="environment">Environment of the configuration to be retrieved.</param>
    /// <param name="component">Component, aka microservice, whose configuration wants to be retrieved.</param>
    /// <param name="workingPath">Path where the configuration will be stored.</param>
    /// <param name="downloadTimeoutInSeconds">Timeout download after specified milliseconds.</param>
    /// <returns><see cref="Client" /> if it is successful, or an <see cref="Error" /> otherwise.</returns>
    public Result<Client, Error> Build(string host, string stage, string environment, string component,
        string workingPath, int downloadTimeoutInMilliseconds);
}