using System;
using System.Threading.Tasks;
using ApiCoverageTool.AssemblyUnderTests.Controllers;
using RestEase;
using Xunit;

namespace ApiCoverageTool.AssemblyUnderTests;

[Trait("Category", "Mock tests")]
public class MockTests
{
    private readonly ITestController _client = RestClient.For<ITestController>();

    [Fact]
    public async Task MockFactAsync()
    {
        await NotTestMethodAsync();
        _ = await _client.GetMethod();
        NotTestMethodNoClientCall();
    }

    [Theory]
    [InlineData(null)]
    public void MockTheory(string str)
    {
        if (string.IsNullOrEmpty(str))
            throw new ArgumentException($"'{nameof(str)}' cannot be null or empty.", nameof(str));

        var someInt = 42;
        var newClient = RestClient.For<ITestController>();
        _ = someInt.ToString() + NotTestMethodNoClientCall();
        _ = newClient.GetMethod().Result;
        NotTestMethodNoClientCall();
    }

    [Theory]
    [InlineData(null)]
    public async Task NoClientCallTheoryAsync(string str)
    {
        if (string.IsNullOrEmpty(str))
            throw new ArgumentException($"'{nameof(str)}' cannot be null or empty.", nameof(str));

        await NotTestMethodAsync();
    }

    [Fact]
    public void NoClientCallFact()
    {
        NotTestMethodNoClientCall();
    }

    [Fact]
    public void MockLambdaExpression()
    {
        NotTestMethodExecutesLambdaExpression(() => _client.PatchAllMethod());
    }

    [Fact]
    public async Task MockLambdaExpressionAsync()
    {
        Func<Task> lambda = async () => await _client.PatchAllMethod();
        await lambda();
    }

    public string NotTestMethodNoClientCall()
    {
        return string.Empty;
    }

    public async Task NotTestMethodAsync()
    {
        await Task.Delay(10);
    }

    private void NotTestMethodExecutesLambdaExpression(Func<Task<object>> lambda)
    {
        _ = lambda().Result;
    }
}
