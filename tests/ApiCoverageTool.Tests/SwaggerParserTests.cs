using System;
using System.Collections.Generic;
using System.IO;
using ApiCoverageTool.Exceptions;
using ApiCoverageTool.Extensions;
using FluentAssertions;
using Xunit;

namespace ApiCoverageTool.Tests
{
    public class SwaggerParserTests
    {
        [Fact]
        public void ParseSwaggerApi_WithNullJson_ThrowsArgumentNullException() =>
            Assert.Throws<ArgumentNullException>(() => SwaggerParser.ParseSwaggerApi(null));

        [Theory]
        [InlineData("")]
        [InlineData("{")]
        [InlineData("{\"}")]
        public void ParseSwaggerApi_WithInvalidJson_ThrowsInvalidSwaggerJsonException(string input) =>
            Assert.Throws<InvalidSwaggerJsonException>(() => SwaggerParser.ParseSwaggerApi(input));

        [Fact]
        public void ParseSwaggerApi_WithEmptySwaggerJson_ThrowsInvalidSwaggerJsonException()
        {
            var json = "{}";
            Assert.Throws<InvalidSwaggerJsonException>(() => SwaggerParser.ParseSwaggerApi(json));
        }

        [Theory]
        [InlineData("noOperationsSwagger")]
        [InlineData("unknownOperationSwagger")]
        public void ParseSwaggerApiFrom_WithInvalidSwaggerJson_ThrowsInvalidSwaggerJsonException(string jsonFileName)
        {
            var jsonPath = Path.Combine("TestData", $"{jsonFileName}.json");
            Assert.Throws<InvalidSwaggerJsonException>(() => SwaggerParser.ParseSwaggerApiFromFile(jsonPath));
        }

        [Fact]
        public void ParseSwaggerApiFrom_WithSwaggerJson_ReturnsListOfOperationsAndEndpoints()
        {
            var expectedEndpoints = new List<string>()
            {
                "GET /api/operation",
                "GET /api/operation/get",
                "GET /api/operation/all",
                "POST /api/operation/all",
                "PUT /api/operation/all",
                "PATCH /api/operation/all",
                "DELETE /api/operation/all"
            };

            var jsonPath = Path.Combine("TestData", "validSwagger.json");
            var endpoints = SwaggerParser.ParseSwaggerApiFromFile(jsonPath).ToMethodPathList();

            endpoints.Should().BeEquivalentTo(expectedEndpoints);
        }
    }
}
