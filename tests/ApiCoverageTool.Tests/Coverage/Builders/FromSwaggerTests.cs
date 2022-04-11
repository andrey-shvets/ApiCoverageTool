using System;
using System.IO;
using ApiCoverageTool.Coverage.Builders;
using ApiCoverageTool.Exceptions;
using Xunit;

namespace ApiCoverageTool.Tests.Coverage.Builders
{
    public class FromSwaggerTests
    {
        [Fact]
        public void FromSwaggerJson_WithNullJson_ThrowsArgumentNullException()
        {
            var builder = RestEaseTestCoverageBuilder.Instance();

            Assert.Throws<ArgumentNullException>(() => builder.FromSwaggerJson(null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("{")]
        [InlineData("INVALID")]
        [InlineData("{ INVALID }")]
        public void FromSwaggerJson_WithInvalidJson_ThrowsInvalidSwaggerJsonException(string json)
        {
            var builder = RestEaseTestCoverageBuilder.Instance();

            Assert.Throws<InvalidSwaggerJsonException>(() => builder.FromSwaggerJson(json));
        }

        [Theory]
        [InlineData("noOperationsSwagger")]
        [InlineData("unknownOperationSwagger")]
        public void FromSwaggerJsonPath_WithInvalidSwaggerJson_ThrowsInvalidSwaggerJsonException(string jsonFileName)
        {
            var builder = RestEaseTestCoverageBuilder.Instance();
            var jsonPath = Path.Combine("TestData", $"{jsonFileName}.json");

            Assert.Throws<InvalidSwaggerJsonException>(() => builder.FromSwaggerJsonPath(jsonPath));
        }
    }
}
