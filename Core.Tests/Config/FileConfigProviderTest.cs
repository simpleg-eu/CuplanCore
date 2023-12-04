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
        ChildObject other = obj as ChildObject;
        if (other is null) return false;

        return A == other.A && B == other.B;
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
        Child other = obj as Child;
        if (other == null) return false;

        return Name == other.Name && Description == other.Description && Object.Equals(other.Object);
    }
}

public class FileConfigProviderTest : TestBase
{
    public FileConfigProviderTest() : base(typeof(FileConfigProviderTest))
    {
    }

    [Fact]
    public async Task Get_ExistingKey_ExpectedValue()
    {
        const int expectedValue = 1234;

        FileConfigProvider configProvider = new(TestDataPath);
        Result<int, Error<string>> result = await configProvider.Get<int>("application.json|Configuration:Nested:Test");

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
        Result<Child, Error<string>> result = await configProvider.Get<Child>("dir1/dir2/example.json|Parent:Child");

        Assert.True(result.IsOk);
        Assert.Equal(expectedValue, result.Unwrap());
    }
}