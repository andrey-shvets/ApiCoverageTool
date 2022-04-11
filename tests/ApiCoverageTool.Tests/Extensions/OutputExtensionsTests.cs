using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using ApiCoverageTool.AssemblyUnderTests.Controllers;
using ApiCoverageTool.Extensions;
using ApiCoverageTool.Models;
using FluentAssertions;
using Xunit;
using static ApiCoverageTool.Coverage.ApiControllerMapping<ApiCoverageTool.RestClient.RestEaseMethodsProcessor>;

namespace ApiCoverageTool.Tests.Extensions
{
    public class OutputExtensionsTests
    {
        [Fact]
        public void ToCsvFile_NullMappedApiResult_ReturnsEmptyString()
        {
            MappedApiResult result = null;
            var fileName = "testCsv.csv";

            result.ToCsvFile(fileName);

            ValidateCsvFile(fileName, string.Empty);
        }

        [Fact]
        public void ToCsvFile_EmptyMappedApiResult_ReturnsEmptyString()
        {
            var result = new MappedApiResult();
            var fileName = "testCsv.csv";

            result.ToCsvFile(fileName);

            ValidateCsvFile(fileName, string.Empty);
        }

        [Fact]
        public void ToCsvFile_MappedForEndpointWithoutTests_ReturnsCsvWithZeroTestCountForEndpoint()
        {
            var result = new MappedApiResult();
            var endpoint = new EndpointInfo(HttpMethod.Get, "endpoint/path");
            result.EndpointsMapping.Add(endpoint, new List<MethodInfo>());

            var expectedCsv = "Method,TestsCount\r\n" +
                              "GET endpoint/path,0\r\n";
            var fileName = "testCsv.csv";

            result.ToCsvFile(fileName);

            ValidateCsvFile(fileName, expectedCsv);
        }

        [Fact]
        public void ToCsvFile_WithRelativePath_CreatesCsvFileWithEndpointCoverageData()
        {
            var jsonPath = Path.Combine("TestData", "coverageTestSwagger.json");
            var result = GetMappingByControllerFromFile(jsonPath, typeof(ITestController));

            var expectedCsv = "Method,TestsCount\r\n" +
                              "GET /api/operation,2\r\n" +
                              "GET /api/operation/all,1\r\n" +
                              "DELETE /api/operation/all,1\r\n" +
                              "POST /api/operation/all/duplicate,2\r\n" +
                              "GET /api/operation/get,1\r\n" +
                              "POST /api/operation/all,0\r\n" +
                              "GET /api/operation/all/duplicate,0\r\n" +
                              "GET /,0\r\n";

            var directoryName = "csvTestDirectory";
            Directory.CreateDirectory(directoryName);

            var fileName = "testCsv.csv";
            var filePath = Path.Combine(directoryName, fileName);

            result.ToCsvFile(filePath);

            ValidateCsvFile(filePath, expectedCsv);
        }

        private static void ValidateCsvFile(string filePath, string expectedCsv)
        {
            var isExistingFile = File.Exists(filePath);

            isExistingFile.Should().BeTrue($"{filePath} file should have been created by ToCsvFile(...) method");

            var csv = File.ReadAllText(filePath);
            csv.Should().Be(expectedCsv);
        }
    }
}
