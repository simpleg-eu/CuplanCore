using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Config;

public class FileConfigProvider : IConfigProvider
{
    private const string KeySeparator = "|";
    private const string SubKeySeparator = ":";

    private readonly Cache _cache;
    private readonly TimeSpan _expireCacheItemAfterTimeSpan;
    private readonly string _targetDirectory;

    public FileConfigProvider(string targetDirectory)
    {
        _targetDirectory = targetDirectory;
        _cache = new Cache(TimeSpan.FromHours(1));
        _expireCacheItemAfterTimeSpan = TimeSpan.FromMinutes(30);
    }

    public void Dispose()
    {
        if (Directory.Exists(_targetDirectory)) Directory.Delete(_targetDirectory, true);

        _cache.Dispose();
    }

    public async Task<Result<T, Error<string>>> Get<T>(string key)
    {
        Result<string, Error<string>> filePathResult = ExtractPathFromKey(key);

        if (!filePathResult.IsOk) return Result<T, Error<string>>.Err(filePathResult.UnwrapErr());

        string filePath = filePathResult.Unwrap();

        Result<object, Empty> cacheResult = _cache.TryGetValue(filePath);

        JObject? jsonObject = null;

        if (cacheResult.IsOk) jsonObject = cacheResult.Unwrap() as JObject;

        if (jsonObject is null)
        {
            if (!File.Exists(filePath))
                return Result<T, Error<string>>.Err(new Error<string>(ErrorKind.NotFound,
                    $"could not find file: {filePath}"));

            string configJson = await File.ReadAllTextAsync(filePath);
            jsonObject = JsonConvert.DeserializeObject<JObject>(configJson);

            if (jsonObject is null)
                return Result<T, Error<string>>.Err(new Error<string>(ErrorKind.InvalidFileContent,
                    "configuration file does not contain a valid JSON object"));

            _cache.Set(filePath, jsonObject, _expireCacheItemAfterTimeSpan);
        }

        Result<string[], Error<string>> subKeysResult = ExtractSubKeysFromKey(key);

        if (!subKeysResult.IsOk) return Result<T, Error<string>>.Err(subKeysResult.UnwrapErr());

        Result<T, Error<string>> valueResult = GetValue<T>(subKeysResult.Unwrap(), jsonObject);

        if (!valueResult.IsOk) return Result<T, Error<string>>.Err(valueResult.UnwrapErr());

        return Result<T, Error<string>>.Ok(valueResult.Unwrap());
    }

    private Result<string, Error<string>> ExtractPathFromKey(string key)
    {
        string[] parts = key.Split(KeySeparator);

        if (parts.Length != 2)
            return Result<string, Error<string>>.Err(new Error<string>(ErrorKind.InvalidArguments,
                $"key does not contain two parts: {key}"));

        return Result<string, Error<string>>.Ok($"{_targetDirectory}/{parts[0]}");
    }

    private Result<string[], Error<string>> ExtractSubKeysFromKey(string key)
    {
        string[] parts = key.Split(KeySeparator);

        if (parts.Length != 2)
            return Result<string[], Error<string>>.Err(new Error<string>(ErrorKind.InvalidArguments,
                $"key does not contain two parts: {key}"));

        string[] subKeys = parts[1].Split(SubKeySeparator);

        return Result<string[], Error<string>>.Ok(subKeys);
    }

    private Result<T, Error<string>> GetValue<T>(string[] subKeys, JObject jsonObject)
    {
        JToken? token = jsonObject;
        foreach (string subKey in subKeys)
        {
            token = token[subKey];

            if (token is null)
                return Result<T, Error<string>>.Err(new Error<string>(ErrorKind.InvalidArguments,
                    $"expected a JToken for subKey '{subKey}' but got null instead"));
        }

        T? value;

        if (token is JObject)
            value = token.ToObject<T>();
        else
            value = token.Value<T>();

        if (value is null)
            return Result<T, Error<string>>.Err(new Error<string>(ErrorKind.InvalidArguments, "failed to get value"));

        return Result<T, Error<string>>.Ok(value);
    }
}