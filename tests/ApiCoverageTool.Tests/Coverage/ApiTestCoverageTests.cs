using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using ApiCoverageTool.AssemblyUnderTests.Controllers;
using ApiCoverageTool.Models;
using FluentAssertions;
using Xunit;
using static ApiCoverageTool.Coverage.ApiTestCoverage<ApiCoverageTool.RestClient.RestEaseMethodsProcessor>;

namespace ApiCoverageTool.Tests.Coverage
{
    public class ApiTestCoverageTests
    {
        private static Assembly AssemblyUnderTest => typeof(AssemblyUnderTests.MockClass).Assembly;

        [Fact]
        public void GetCoverage_WithNullServiceEndpoints_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => GetTestCoverage(null, Array.Empty<Type>()));
        }

        [Fact]
        public void GetCoverage_WithNullMappedEndpoints_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentNullException>(() => GetTestCoverage(Assembly.GetCallingAssembly(), null));
        }

        [Fact]
        public void GetCoverage_ProvidedAssemblyWithTests_ReturnsListOfEndpointsCoveredWithTests()
        {
            var expectedMapped = new List<(EndpointInfo Endpoint, List<string> Methods)>
            {
                (new EndpointInfo(HttpMethod.Get, "/api/operation/get"), new List<string> { "MockFactAsync", "MockTheory", "MockTestNestedCallAsync", "MockTestNestedCall", "MockTestWithCycleInCallTree" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation"), new List<string> { "MockTestNestedCallAsync" }),
                (new EndpointInfo(HttpMethod.Patch, "/api/operation/all"), new List<string> { "MockTestNestedCall", "MockLambdaExpression", "MockLambdaExpressionAsync" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/all"), new List<string> { "MockTestDifferentClassStaticCall", "MockTestDifferentClassCall" })
            };

            var result = GetTestCoverage(AssemblyUnderTest, typeof(ITestController));

            ValidateMappedEndpoints(result, expectedMapped);
        }

        private static void ValidateMappedEndpoints(Dictionary<EndpointInfo, List<MethodInfo>> mappedEndpoints, List<(EndpointInfo Endpoint, List<string> Methods)> expected)
        {
            var actual = mappedEndpoints.Keys.Select(e => (e, mappedEndpoints is null ? null : ToStringList(mappedEndpoints[e]))).ToList();

            actual.Should().BeEquivalentTo(expected);
        }

        private static IList<string> ToStringList(List<MethodInfo> methods)
        {
            return methods?.Select(m => m.Name).ToList();
        }
    }
}
