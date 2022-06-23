using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using ApiCoverageTool.AssemblyUnderTests.Controllers;
using ApiCoverageTool.Models;
using ApiCoverageTool.Tests.Helpers;
using Xunit;
using static ApiCoverageTool.Coverage.ControllerMethodsTestCoverage<ApiCoverageTool.RestClient.RestEaseMethodsProcessor>;

namespace ApiCoverageTool.Tests.Coverage
{
    public class ControllerMethodsTestCoverageTests
    {
        private static Assembly AssemblyUnderTest => typeof(AssemblyUnderTests.MockClass).Assembly;

        [Fact]
        public void GetCoverage_WithNullServiceEndpoints_ThrowsArgumentException() => Assert.Throws<ArgumentNullException>(() => GetTestCoverage(null, Array.Empty<Type>()));

        [Fact]
        public void GetCoverage_WithNullMappedEndpoints_ThrowsArgumentException() => Assert.Throws<ArgumentNullException>(() => GetTestCoverage(Assembly.GetCallingAssembly(), null));

        [Fact]
        public void GetCoverage_ProvidedAssemblyWithTests_ReturnsListOfEndpointsCoveredWithTests()
        {
            var expectedMapped = new List<(EndpointInfo Endpoint, List<string> Methods)>
            {
                (new EndpointInfo(HttpMethod.Get, "/api/operation/get"), new List<string> { "MockFactAsync", "MockTheory", "MockTestNestedCallAsync", "MockTestNestedCall", "MockTestWithCycleInCallTree" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation"), new List<string> { "MockTestNestedCallAsync" }),
                (new EndpointInfo(HttpMethod.Patch, "/api/operation/all"), new List<string> { "MockTestNestedCall", "MockLambdaExpression", "MockLambdaExpressionAsync" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/all"), new List<string> { "MockTestDifferentClassStaticCall", "MockTestDifferentClassCall" }),
                (new EndpointInfo(HttpMethod.Put, "/api/operation/withparameters"), new List<string> { "MockTestEndpointWithParameters" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/with/{somepath}/parameter"), new List<string> { "MockTestCallEndpointWithPathParameter" })
            };

            var result = GetTestCoverage(AssemblyUnderTest, typeof(ITestController));

            result.ValidateMappedEndpoints(expectedMapped);
        }
    }
}
