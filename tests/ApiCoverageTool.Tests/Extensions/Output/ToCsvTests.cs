using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using ApiCoverageTool.AssemblyUnderTests.Controllers;
using ApiCoverageTool.Coverage.Builders;
using ApiCoverageTool.Extensions;
using ApiCoverageTool.Models;
using FluentAssertions;
using Xunit;

namespace ApiCoverageTool.Tests.Extensions.Output;

public class ToCsvTests
{
    private const string FileName = "testCsv.csv";
    private static string LineBreak { get; } = "\r\n";

    [Fact]
    public void ToCsv_NullMappedApiResult_ReturnsEmptyString()
    {
        ApiCoverageResult coverageResult = null;

        Assert.Throws<ArgumentNullException>(() => coverageResult.ToCsv(FileName));
    }

    [Fact]
    public void ToCsv_EmptyMappedApiResult_ReturnsEmptyString()
    {
        var result = new ApiCoverageResult();

        result.ToCsv(FileName);

        ValidateCsvFile(FileName, "Method,Endpoint,TestsCount\r\n");
    }

    [Fact]
    public void ToCsv_MappedForEndpointWithoutTests_ReturnsCsvWithZeroTestCountForEndpoint()
    {
        var result = new ApiCoverageResult();
        var endpoint = new EndpointInfo(HttpMethod.Get, "endpoint/path");
        result.EndpointsMapping.Add(endpoint, new List<MethodBase>());

        var expectedCsv = "Method,Endpoint,TestsCount\r\n" +
                          "GET,endpoint/path,0\r\n";

        result.ToCsv(FileName);

        ValidateCsvFile(FileName, expectedCsv);
    }

    [Fact]
    public void ToCsv_WithRelativePath_CreatesCsvFileWithEndpointCoverageData()
    {
        var assemblyUnderTest = typeof(AssemblyUnderTests.MockClass).Assembly;
        var jsonPath = Path.Combine("TestData", "coverageTestSwagger.json");

        var result = RestEaseTestCoverageBuilder
            .ForTestsInAssembly(assemblyUnderTest)
            .ForController<ITestController>()
            .UseSwaggerJsonPath(jsonPath)
            .ApiTestCoverage;

        var expectedCsv = $"Method,Endpoint,TestsCount{LineBreak}" +
                          $"GET,/api/operation,1{LineBreak}" +
                          $"GET,/api/operation/all,2{LineBreak}" +
                          $"PATCH,/api/operation/all,3{LineBreak}" +
                          $"GET,/api/operation/get,5{LineBreak}" +
                          $"GET,/api/operation/with/{{path}}/parameter,1{LineBreak}" +
                          $"PUT,/api/operation/withparameters,1{LineBreak}" +
                          $"POST,/api/operation/all,0{LineBreak}" +
                          $"DELETE,/api/operation/all,0{LineBreak}" +
                          $"POST,/api/operation/all/duplicate,0{LineBreak}" +
                          $"GET,/api/operation/all/duplicate,0{LineBreak}" +
                          $"PUT,/api/operation/withparametersnottested,0{LineBreak}";

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
