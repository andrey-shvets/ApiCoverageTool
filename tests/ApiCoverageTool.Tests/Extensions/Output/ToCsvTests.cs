using System;
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

namespace ApiCoverageTool.Tests.Extensions.Output
{
    public class ToCsvTests
    {
        private const string FileName = "testCsv.csv";

        [Fact]
        public void ToCsv_NullMappedApiResult_ReturnsEmptyString()
        {
            MappedApiResult result = null;

            Assert.Throws<ArgumentNullException>(() => result.ToCsv(FileName));
        }

        [Fact]
        public void ToCsv_EmptyMappedApiResult_ReturnsEmptyString()
        {
            var result = new MappedApiResult();

            result.ToCsv(FileName);

            ValidateCsvFile(FileName, "Method,Endpoint,TestsCount\r\n");
        }

        [Fact]
        public void ToCsv_MappedForEndpointWithoutTests_ReturnsCsvWithZeroTestCountForEndpoint()
        {
            var result = new MappedApiResult();
            var endpoint = new EndpointInfo(HttpMethod.Get, "endpoint/path");
            result.EndpointsMapping.Add(endpoint, new List<MethodInfo>());

            var expectedCsv = "Method,Endpoint,TestsCount\r\n" +
                              "GET,endpoint/path,0\r\n";

            result.ToCsv(FileName);

            ValidateCsvFile(FileName, expectedCsv);
        }

        [Fact]
        public void ToCsv_WithRelativePath_CreatesCsvFileWithEndpointCoverageData()
        {
            var jsonPath = Path.Combine("TestData", "coverageTestSwagger.json");
            var result = GetMappingByControllerFromFile(jsonPath, typeof(ITestController));

            var expectedCsv = "Method,Endpoint,TestsCount\r\n" +
                              "GET,/api/operation,2\r\n" +
                              "GET,/api/operation/all,1\r\n" +
                              "DELETE,/api/operation/all,1\r\n" +
                              "POST,/api/operation/all/duplicate,2\r\n" +
                              "GET,/api/operation/get,1\r\n" +
                              "POST,/api/operation/all,0\r\n" +
                              "GET,/api/operation/all/duplicate,0\r\n";

            var directoryName = "csvTestDirectory";
            Directory.CreateDirectory(directoryName);

            var filePath = Path.Combine(directoryName, FileName);

            result.ToCsv(filePath);

            ValidateCsvFile(filePath, expectedCsv);
        }

        private static void ValidateCsvFile(string filePath, string expectedCsv)
        {
            var isExistingFile = File.Exists(filePath);

            isExistingFile.Should().BeTrue($"{filePath} file should have been created by ToCsv(...) method");

            var csv = File.ReadAllText(filePath);
            csv.Should().Be(expectedCsv);
        }
    }
}
