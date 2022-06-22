using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using ApiCoverageTool.AssemblyUnderTests.Controllers;
using ApiCoverageTool.Coverage.Builders;
using ApiCoverageTool.Extensions;
using ApiCoverageTool.Models;
using ApiCoverageTool.Tests.Helpers;
using ClosedXML.Excel;
using FluentAssertions;
using Xunit;

namespace ApiCoverageTool.Tests.Extensions.Output;

public class ToXlsxTests
{
    private const string FileName = "testXlsx.xlsx";
    private const string SheetName = "testApi";

    private static string LineBreak { get; } = Environment.NewLine;

    public ToXlsxTests()
    {
        if (File.Exists(FileName))
            File.Delete(FileName);
    }

    [Fact]
    public void ToXlsx_TargetFileHasWrongExtension_ThrowsArgumentException()
    {
        var result = new ApiCoverageResult();
        Assert.Throws<ArgumentException>(() => result.ToXlsx("test.csv", "testApi"));
    }

    [Fact]
    public void ToXlsx_NullMappedApiResult_ReturnsEmptyString()
    {
        ApiCoverageResult coverageResult = null;

        Assert.Throws<ArgumentNullException>(() => coverageResult.ToXlsx(FileName, "testApi"));
    }

    [Fact]
    public void ToXlsx_EmptyMappedApiResult_ReturnsEmptyString()
    {
        var result = new ApiCoverageResult();

        result.ToXlsx(FileName, SheetName);

        ValidateXlsxFile(FileName, SheetName, $"Method,Endpoint,Tests count{LineBreak}");
    }

    [Fact]
    public void ToXlsx_MappedForEndpointWithoutTests_ReturnsCsvWithZeroTestCountForEndpoint()
    {
        var result = new ApiCoverageResult();
        var endpoint = new EndpointInfo(HttpMethod.Get, "/endpoint/path");
        result.EndpointsMapping.Add(endpoint, new List<MethodInfo>());

        var expectedCsv = $"Method,Endpoint,Tests count{LineBreak}" +
                          $"GET,/endpoint/path,0{LineBreak}";

        result.ToXlsx(FileName, SheetName);

        ValidateXlsxFile(FileName, SheetName, expectedCsv);
    }

    [Fact]
    public void ToXlsx_WithRelativePath_CreatesCsvFileWithEndpointCoverageData()
    {
        var assemblyUnderTest = typeof(AssemblyUnderTests.MockClass).Assembly;
        var jsonPath = Path.Combine("TestData", "coverageTestSwagger.json");

        var result = RestEaseTestCoverageBuilder
            .ForTestsInAssembly(assemblyUnderTest)
            .ForController<ITestController>()
            .UseSwaggerJsonPath(jsonPath)
            .ApiCoverageTestCoverage;

        var expectedCsv = $"Method,Endpoint,Tests count{LineBreak}" +
                          $"GET,/api/operation/get,5{LineBreak}" +
                          $"PATCH,/api/operation/all,3{LineBreak}" +
                          $"GET,/api/operation,1{LineBreak}" +
                          $"GET,/api/operation/all,2{LineBreak}" +
                          $"PUT,/api/operation/withparameters,1{LineBreak}" +
                          $"POST,/api/operation/all,0{LineBreak}" +
                          $"DELETE,/api/operation/all,0{LineBreak}" +
                          $"POST,/api/operation/all/duplicate,0{LineBreak}" +
                          $"GET,/api/operation/all/duplicate,0{LineBreak}" +
                          $"PUT,/api/operation/withparametersnottested,0{LineBreak}";

        result.ToXlsx(FileName, SheetName);

        ValidateXlsxFile(FileName, SheetName, expectedCsv);
    }

    [Fact]
    public void ToXlsx_CreatesNewWorksheet_IfProvidedXlsFileExists()
    {
        var result = new ApiCoverageResult();
        var endpoint = new EndpointInfo(HttpMethod.Get, "/endpoint/path");
        result.EndpointsMapping.Add(endpoint, new List<MethodInfo>());

        var expectedCsv = $"Method,Endpoint,Tests count{LineBreak}" +
                          $"GET,/endpoint/path,0{LineBreak}";

        result.ToXlsx(FileName, SheetName);

        var emptyResult = new ApiCoverageResult();
        var emptyWorksheetName = "emptyWorksheet";
        emptyResult.ToXlsx(FileName, emptyWorksheetName);

        ValidateXlsxFile(FileName, SheetName, expectedCsv);
        ValidateXlsxFile(FileName, emptyWorksheetName, $"Method,Endpoint,Tests count{LineBreak}");
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
