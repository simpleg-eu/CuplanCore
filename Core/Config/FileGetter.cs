using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Core.Config;

public class FileGetter : IGetter
{
    private const string KeySeparator = ":";

    private readonly Cache _cache;
    private readonly TimeSpan _expireCacheItemAfter;
    private readonly string _targetDirectory;

    public FileGetter(string targetDirectory, Cache cache, TimeSpan expireCacheItemAfter)
    {
        _targetDirectory = targetDirectory;
        _cache = cache;
        _expireCacheItemAfter = expireCacheItemAfter;
    }

    public void Dispose()
    {
        if (Directory.Exists(_targetDirectory)) Directory.Delete(_targetDirectory, true);

        _cache.Dispose();
    }

    public async Task<Result<T, Error>> Get<T>(string filePath, string key)
    {
        filePath = $"{_targetDirectory}/{filePath}";
        var cacheResult = _cache.TryGetValue(filePath);

        JObject? configObject = null;

        if (cacheResult.IsOk) configObject = cacheResult.Unwrap() as JObject;

        if (configObject is null)
        {
            if (!File.Exists(filePath))
                return Result<T, Error>.Err(new Error(ErrorKind.NotFound,
                    $"could not find file: {filePath}"));

            var configYaml = await File.ReadAllTextAsync(filePath);
            var deserializer =
                new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var yamlObject = deserializer.Deserialize(configYaml);

            if (yamlObject is null)
                return Result<T, Error>.Err(new Error(ErrorKind.InvalidFileContent,
                    "invalid file content"));

            // YAML conversion to JSON because Microconfig works with YAML.
            var serializer = new SerializerBuilder().JsonCompatible().Build();

            var json = serializer.Serialize(yamlObject);

            configObject = JObject.Parse(json);

            _cache.Set(filePath, configObject, _expireCacheItemAfter);
        }

        var subKeys = key.Split(KeySeparator);

        var valueResult = GetValue<T>(subKeys, configObject);

        if (!valueResult.IsOk) return Result<T, Error>.Err(valueResult.UnwrapErr());

        return Result<T, Error>.Ok(valueResult.Unwrap());
    }

    public void CleanCache()
    {
        _cache.Clear();
    }

    private static Result<T, Error> GetValue<T>(IEnumerable<string> subKeys, JObject jsonObject)
    {
        JToken? token = jsonObject;
        foreach (var subKey in subKeys)
        {
            token = token[subKey];

            if (token is null)
                return Result<T, Error>.Err(new Error(ErrorKind.InvalidArguments,
                    $"expected a JToken for subKey '{subKey}' but got null instead"));
        }

        T? value;

        if (token is JObject || token is JArray)
            value = token.ToObject<T>();
        else
            value = token.Value<T>();

        if (value is null)
            return Result<T, Error>.Err(new Error(ErrorKind.InvalidArguments, "failed to get value"));

        return Result<T, Error>.Ok(value);
    }
}