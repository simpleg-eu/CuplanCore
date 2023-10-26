using Core;
using Xunit;

namespace Cuplan.Core.Tests;

public class ResultTest
{
    [Fact]
    public void Ok_CreatesOkResult()
    {
        const int expectedNumber = 2;


        Result<int, string> result = Result<int, string>.Ok(expectedNumber);


        Assert.True(result.IsOk);
        Assert.True(result.Match(
            i => { return expectedNumber == i; }, s =>
            {
                Assert.Fail();
                return false;
            }
        ));
    }

    [Fact]
    public void Err_CreatesErrorResult()
    {
        const string expectedString = "abcd";


        Result<int, string> result = Result<int, string>.Err(expectedString);


        Assert.False(result.IsOk);
        Assert.Equal(expectedString, result.Match(i =>
        {
            Assert.Fail();
            return "";
        }, s => { return s; }));
    }
}