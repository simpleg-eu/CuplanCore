using Core;
using Core.Config;
using Xunit;

namespace Cuplan.Core.Tests.Config;

internal class ChildObject
{
    public int A { get; set; }
    public int B { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        var other = obj as ChildObject;
        if (other is null) return false;

        return A == other.A && B == other.B;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(A, B);
    }
}

internal class Child
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ChildObject Object { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        var other = obj as Child;
        if (other == null) return false;

        return Name == other.Name && Description == other.Description && Object.Equals(other.Object);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Description, Object);
    }
}

public class FileGetterTest : TestBase
{
    private readonly Cache _cache;
    private readonly TimeSpan _cacheExpireItemAfter;
    private readonly FileGetter _getter;

    public FileGetterTest()
    {
        _cache = new Cache(TimeSpan.FromHours(1));
        _cacheExpireItemAfter = TimeSpan.FromMinutes(30);
        _getter = new FileGetter(TestDataPath, _cache, _cacheExpireItemAfter);
    }

    [Fact]
    public async Task Get_ExistingKey_ExpectedValue()
    {
        const int expectedValue = 1234;

        var result = await _getter.Get<int>("application.yaml", "Configuration:Nested:Test");

        Assert.True(result.IsOk);
        Assert.Equal(expectedValue, result.Unwrap());
    }

    [Fact]
    public async Task Get_ExistingComplexKey_ExpectedValue()
    {
        Child expectedValue = new()
        {
            Name = "Alpha",
            Description = "Whatever",
            Object = new ChildObject
            {
                A = 2,
                B = 4
            }
        };

        var result = await _getter.Get<Child>("dir1/dir2/example.yaml", "Parent:Child");

        Assert.True(result.IsOk);
        Assert.Equal(expectedValue, result.Unwrap());
    }

    [Fact]
    public async Task Get_ExistingComplexKeyThatPointsToArray_ExpectedValue()
    {
        string[] origins =
        {
            "http://localhost:4200",
            "https://cuplan.simpleg.eu"
        };

        var
            result = await _getter.Get<string[]>("other/new/array.yaml", "Cors:Origins");

        Assert.True(result.IsOk);
        Assert.True(origins.SequenceEqual(result.Unwrap()));
    }

    [Fact]
    public void Reset_CleansCache()
    {
        const string key = "EXAMPLE";
        const string value = "HAHAHA";
        _cache.Set(key, value, _cacheExpireItemAfter);

        _getter.CleanCache();

        Assert.True(_cache.IsEmpty);
    }
}