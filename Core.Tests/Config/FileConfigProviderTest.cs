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
        ChildObject? other = obj as ChildObject;
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
        Child? other = obj as Child;
        if (other == null) return false;

        return Name == other.Name && Description == other.Description && Object.Equals(other.Object);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Description, Object);
    }
}

public class FileConfigProviderTest : TestBase
{
    [Fact]
    public async Task Get_ExistingKey_ExpectedValue()
    {
        const int expectedValue = 1234;

        FileConfigProvider configProvider = new(TestDataPath);
        Result<int, Error> result = await configProvider.Get<int>("application.yaml|Configuration:Nested:Test");

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

        FileConfigProvider configProvider = new(TestDataPath);
        Result<Child, Error> result = await configProvider.Get<Child>("dir1/dir2/example.yaml|Parent:Child");

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

        FileConfigProvider configProvider = new(TestDataPath);
        Result<string[], Error>
            result = await configProvider.Get<string[]>("other/new/array.yaml|Cors:Origins");

        Assert.True(result.IsOk);
        Assert.True(origins.SequenceEqual(result.Unwrap()));
    }
}