using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using ApiCoverageTool.AssemblyUnderTests.Controllers;
using ApiCoverageTool.Coverage.Builders;
using ApiCoverageTool.Models;
using ApiCoverageTool.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace ApiCoverageTool.Tests.Coverage.Builders
{
    public class ApiTestCoverageTests
    {
        private static Assembly AssemblyUnderTest => typeof(AssemblyUnderTests.MockClass).Assembly;

        [Fact]
        public void ApiByControllerCoverage_WithSwaggerJson_ReturnsListOfOperationsAndEndpoints()
        {
            var expectedNotCoveredEndpoints = new List<EndpointInfo>()
            {
                new EndpointInfo(HttpMethod.Post, "/api/operation/all"),
                new EndpointInfo(HttpMethod.Get, "/api/operation/all/duplicate")
            };

            var expectedCoveredEndpoints = new List<(EndpointInfo Endpoint, List<string> Methods)>
            {
                (new EndpointInfo(HttpMethod.Get, "/api/operation"), new List<string> { "GetNoPathMethod", "GetEmptyPathMethod" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/all"), new List<string> { "GetAllMethod" }),
                (new EndpointInfo(HttpMethod.Delete, "/api/operation/all"), new List<string> { "DeleteAllMethod" }),
                (new EndpointInfo(HttpMethod.Patch, "/api/operation/all"), new List<string> { "PatchAllMethod" }),
                (new EndpointInfo(HttpMethod.Post, "/api/operation/all/duplicate"), new List<string> { "PostMethod", "PostDuplicateMethod" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/get"), new List<string> { "GetMethod" }),
                (new EndpointInfo(HttpMethod.Put, "/api/operation/withparameters"), new List<string> { "CallEndpointWithParameters" }),
                (new EndpointInfo(HttpMethod.Put, "/api/operation/withparametersnottested"), new List<string> { "CallEndpointWithParametersNotTested" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/with/{path}/parameter"), new List<string> { "CallEndpointWithPathParameter" })
            };

            var expectedEndpoints = new List<(EndpointInfo Endpoint, List<string> Methods)>
            {
                (new EndpointInfo(HttpMethod.Get, "/api/operation"), new List<string> { "GetNoPathMethod", "GetEmptyPathMethod" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/all"), new List<string> { "GetAllMethod" }),
                (new EndpointInfo(HttpMethod.Delete, "/api/operation/all"), new List<string> { "DeleteAllMethod" }),
                (new EndpointInfo(HttpMethod.Patch, "/api/operation/all"), new List<string> { "PatchAllMethod" }),
                (new EndpointInfo(HttpMethod.Post, "/api/operation/all/duplicate"), new List<string> { "PostMethod", "PostDuplicateMethod" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/get"), new List<string> { "GetMethod" }),
                (new EndpointInfo(HttpMethod.Put, "/api/operation/withparameters"), new List<string> { "CallEndpointWithParameters" }),
                (new EndpointInfo(HttpMethod.Put, "/api/operation/withparametersnottested"), new List<string> { "CallEndpointWithParametersNotTested" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/with/{path}/parameter"), new List<string> { "CallEndpointWithPathParameter" }),
                (new EndpointInfo(HttpMethod.Post, "/api/operation/all"), new List<string>()),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/all/duplicate"), new List<string>())
            };

            var jsonPath = Path.Combine("TestData", "coverageTestSwagger.json");

            var builder = RestEaseTestCoverageBuilder
                .ForTestsInAssembly(AssemblyUnderTest)
                .ForController<ITestController>()
                .UseSwaggerJsonPath(jsonPath);

            var result = builder.ApiCoverageByController;

            result.EndpointsMapping.ValidateMappedEndpoints(expectedEndpoints);
            result.NotCoveredEndpoints.Should().BeEquivalentTo(expectedNotCoveredEndpoints);
            result.CoveredEndpoints.ValidateMappedEndpoints(expectedCoveredEndpoints);
        }

        [Fact]
        public void ApiTestCoverage_WithSwaggerJson_ReturnsListOfOperationsAndEndpoints()
        {
            var expectedNotCoveredEndpoints = new List<EndpointInfo>()
            {
                new EndpointInfo(HttpMethod.Post, "/api/operation/all"),
                new EndpointInfo(HttpMethod.Delete, "/api/operation/all"),
                new EndpointInfo(HttpMethod.Post, "/api/operation/all/duplicate"),
                new EndpointInfo(HttpMethod.Get, "/api/operation/all/duplicate"),
                new EndpointInfo(HttpMethod.Put, "/api/operation/withparametersnottested")
            };

            var expectedCoveredEndpoints = new List<(EndpointInfo Endpoint, List<string> Methods)>
            {
                (new EndpointInfo(HttpMethod.Get, "/api/operation"), new List<string> { "MockTestNestedCallAsync" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/all"), new List<string> { "MockTestDifferentClassStaticCall", "MockTestDifferentClassCall" }),
                (new EndpointInfo(HttpMethod.Patch, "/api/operation/all"), new List<string> { "MockLambdaExpression", "MockLambdaExpressionAsync", "MockTestNestedCall" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/get"), new List<string> { "MockFactAsync", "MockTheory", "MockTestNestedCallAsync", "MockTestNestedCall", "MockTestWithCycleInCallTree" }),
                (new EndpointInfo(HttpMethod.Put, "/api/operation/withparameters"), new List<string> { "MockTestEndpointWithParameters" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/with/{path}/parameter"), new List<string> { "MockTestCallEndpointWithPathParameter" })
            };

            var expectedEndpoints = new List<(EndpointInfo Endpoint, List<string> Methods)>
            {
                (new EndpointInfo(HttpMethod.Get, "/api/operation"), new List<string> { "MockTestNestedCallAsync" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/all"), new List<string> { "MockTestDifferentClassStaticCall", "MockTestDifferentClassCall" }),
                (new EndpointInfo(HttpMethod.Patch, "/api/operation/all"), new List<string> { "MockLambdaExpression", "MockLambdaExpressionAsync", "MockTestNestedCall" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/get"), new List<string> { "MockFactAsync", "MockTheory", "MockTestNestedCallAsync", "MockTestNestedCall", "MockTestWithCycleInCallTree" }),
                (new EndpointInfo(HttpMethod.Put, "/api/operation/withparameters"), new List<string> { "MockTestEndpointWithParameters" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/with/{path}/parameter"), new List<string> { "MockTestCallEndpointWithPathParameter" }),
                (new EndpointInfo(HttpMethod.Post, "/api/operation/all"), new List<string>()),
                (new EndpointInfo(HttpMethod.Delete, "/api/operation/all"), new List<string>()),
                (new EndpointInfo(HttpMethod.Post, "/api/operation/all/duplicate"), new List<string>()),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/all/duplicate"), new List<string>()),
                (new EndpointInfo(HttpMethod.Put, "/api/operation/withparametersnottested"),  new List<string>())
            };

            var jsonPath = Path.Combine("TestData", "coverageTestSwagger.json");

            var builder = RestEaseTestCoverageBuilder
                .ForTestsInAssembly(AssemblyUnderTest)
                .ForController<ITestController>()
                .UseSwaggerJsonPath(jsonPath);

            var result = builder.ApiTestCoverage;

            result.EndpointsMapping.ValidateMappedEndpoints(expectedEndpoints);
            result.NotCoveredEndpoints.Should().BeEquivalentTo(expectedNotCoveredEndpoints);
            result.CoveredEndpoints.ValidateMappedEndpoints(expectedCoveredEndpoints);
        }
    }
}
