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

    [Fact]
    public void Unwrap_ReturnsOkResult()
    {
        const string expectedString = "abcd";


        Result<string, int> result = Result<string, int>.Ok(expectedString);


        Assert.Equal(expectedString, result.Unwrap());
    }

    [Fact]
    public void Unwrap_ErrResult_ThrowsException()
    {
        Result<string, int> result = Result<string, int>.Err(1);


        Assert.Throws<InvalidOperationException>(result.Unwrap);
    }

    [Fact]
    public void UnwrapErr_ReturnsErrResult()
    {
        const string expectedString = "abcd";


        Result<int, string> result = Result<int, string>.Err(expectedString);


        Assert.Equal(expectedString, result.UnwrapErr());
    }

    [Fact]
    public void UnwrapErr_OkResult_ThrowsException()
    {
        Result<int, string> result = Result<int, string>.Ok(1);


        Assert.Throws<InvalidOperationException>(result.UnwrapErr);
    }
}