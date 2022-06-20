using System;
using System.IO;
using System.Reflection;
using ApiCoverageTool.Coverage.Builders;
using ApiCoverageTool.Exceptions;
using Xunit;

namespace ApiCoverageTool.Tests.Coverage.Builders;

public class UseSwaggerTests
{
    private static Assembly AssemblyUnderTest => typeof(AssemblyUnderTests.MockClass).Assembly;

    [Fact]
    public void UseSwaggerJson_WithNullJson_ThrowsArgumentNullException()
    {
        var builder = RestEaseTestCoverageBuilder.ForTestsInAssembly(AssemblyUnderTest);

        Assert.Throws<ArgumentNullException>(() => builder.UseSwaggerJson(null));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("{")]
    [InlineData("INVALID")]
    [InlineData("{ INVALID }")]
    public void UseSwaggerJson_WithInvalidJson_ThrowsInvalidSwaggerJsonException(string json)
    {
        var builder = RestEaseTestCoverageBuilder.ForTestsInAssembly(AssemblyUnderTest);

        Assert.Throws<InvalidSwaggerJsonException>(() => builder.UseSwaggerJson(json));
    }

    [Theory]
    [InlineData("noOperationsSwagger")]
    [InlineData("unknownOperationSwagger")]
    public void UseSwaggerJsonPath_WithInvalidSwaggerJson_ThrowsInvalidSwaggerJsonException(string jsonFileName)
    {
        var builder = RestEaseTestCoverageBuilder.ForTestsInAssembly(AssemblyUnderTest);
        var jsonPath = Path.Combine("TestData", $"{jsonFileName}.json");

        Assert.Throws<InvalidSwaggerJsonException>(() => builder.UseSwaggerJsonPath(jsonPath));
    }
}
