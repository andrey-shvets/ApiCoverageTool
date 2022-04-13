using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using ApiCoverageTool.AssemblyUnderTests.Controllers;
using ApiCoverageTool.Extensions;
using ApiCoverageTool.Models;
using ApiCoverageTool.Tests.Helpers;
using ClosedXML.Excel;
using FluentAssertions;
using Xunit;
using static ApiCoverageTool.Coverage.ApiControllerMapping<ApiCoverageTool.RestClient.RestEaseMethodsProcessor>;

namespace ApiCoverageTool.Tests.Extensions.Output
{
    public class ToXlsxTests
    {
        private const string FileName = "testXlsx.xlsx";
        private const string SheetName = "testApi";

        [Fact]
        public void ToXlsx_TargetFileHasWrongExtension_ThrowsArgumentException()
        {
            var result = new MappedApiResult();
            Assert.Throws<ArgumentException>(() => result.ToXlsx("test.csv", "testApi"));
        }

        [Fact]
        public void ToXlsx_NullMappedApiResult_ReturnsEmptyString()
        {
            MappedApiResult result = null;

            Assert.Throws<ArgumentNullException>(() => result.ToXlsx(FileName, "testApi"));
        }

        [Fact]
        public void ToXlsx_EmptyMappedApiResult_ReturnsEmptyString()
        {
            var result = new MappedApiResult();

            result.ToXlsx(FileName, SheetName);

            ValidateXlsxFile(FileName, SheetName, "Method,Endpoint,Tests count\r\n");
        }

        [Fact]
        public void ToXlsx_MappedForEndpointWithoutTests_ReturnsCsvWithZeroTestCountForEndpoint()
        {
            var result = new MappedApiResult();
            var endpoint = new EndpointInfo(HttpMethod.Get, "endpoint/path");
            result.EndpointsMapping.Add(endpoint, new List<MethodInfo>());

            var expectedCsv = "Method,Endpoint,Tests count\r\n" +
                              "GET,endpoint/path,0\r\n";

            result.ToXlsx(FileName, SheetName);

            ValidateXlsxFile(FileName, SheetName, expectedCsv);
        }

        [Fact]
        public void ToXlsx_WithRelativePath_CreatesCsvFileWithEndpointCoverageData()
        {
            var jsonPath = Path.Combine("TestData", "coverageTestSwagger.json");
            var result = GetMappingByControllerFromFile(jsonPath, typeof(ITestController));

            var expectedCsv = "Method,Endpoint,Tests count\r\n" +
                              "GET,/api/operation,2\r\n" +
                              "GET,/api/operation/all,1\r\n" +
                              "DELETE,/api/operation/all,1\r\n" +
                              "POST,/api/operation/all/duplicate,2\r\n" +
                              "GET,/api/operation/get,1\r\n" +
                              "POST,/api/operation/all,0\r\n" +
                              "GET,/api/operation/all/duplicate,0\r\n";

            var directoryName = "xlsxTestDirectory";
            Directory.CreateDirectory(directoryName);

            var filePath = Path.Combine(directoryName, FileName);

            result.ToXlsx(filePath, SheetName);

            ValidateXlsxFile(filePath, SheetName, expectedCsv);
        }

        [Fact]
        public void ToXlsx_CreatesNewWorksheet_IfProvidedXlsFileExists()
        {
            var result = new MappedApiResult();
            var endpoint = new EndpointInfo(HttpMethod.Get, "endpoint/path");
            result.EndpointsMapping.Add(endpoint, new List<MethodInfo>());

            var expectedCsv = "Method,Endpoint,Tests count\r\n" +
                              "GET,endpoint/path,0\r\n";

            var directoryName = "xlsxTestDirectory";
            Directory.CreateDirectory(directoryName);

            var filePath = Path.Combine(directoryName, FileName);

            result.ToXlsx(filePath, SheetName);

            var emptyResult = new MappedApiResult();
            var emptyWorksheetName = "emptyWorksheet";
            emptyResult.ToXlsx(filePath, emptyWorksheetName);

            ValidateXlsxFile(filePath, SheetName, expectedCsv);
            ValidateXlsxFile(filePath, emptyWorksheetName, "Method,Endpoint,Tests count\r\n");
        }

        private static void ValidateXlsxFile(string filePath, string worksheetName, string expectedCsv)
        {
            var isExistingFile = File.Exists(filePath);
            isExistingFile.Should().BeTrue($"{filePath} file should have been created by ToXlsx(...) method");

            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(worksheetName);
            var lastRowUsedIndex = worksheet.LastRowUsed().RowNumber();
            var range = worksheet.Range($"A1:C{lastRowUsedIndex}");
            var csv = range.ToCsvString();

            csv.Should().Be(expectedCsv);
        }
    }
}
