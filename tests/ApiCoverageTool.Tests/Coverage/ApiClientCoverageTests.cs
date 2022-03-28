using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using ApiCoverageTool.AssemblyUnderTests.Controllers;
using ApiCoverageTool.Models;
using FluentAssertions;
using Xunit;
using static ApiCoverageTool.Coverage.ApiClientCoverage<ApiCoverageTool.RestClient.RestEaseMethodsProcessor>;

namespace ApiCoverageTool.Tests.Coverage
{
    public class ApiClientCoverageTests
    {
        [Fact]
        public void GetCoverageByClient_WithNullServiceEndpoints_ThrowsArgumentException() =>
            Assert.Throws<ArgumentNullException>(() => GetCoverageByClient(null, new List<MappedEndpointInfo>()));

        [Fact]
        public void GetCoverageByClient_WithNullMappedEndpoints_ThrowsArgumentException() =>
            Assert.Throws<ArgumentNullException>(() => GetCoverageByClient(new List<EndpointInfo>(), null));

        [Fact]
        public void GetCoverageByClientFromFile_WithSwaggerJson_ReturnsListOfOperationsAndEndpoints()
        {
            var expectedNotMapped = new List<EndpointInfo>()
            {
                new EndpointInfo(HttpMethod.Post, "/api/operation/all"),
                new EndpointInfo(HttpMethod.Get, "/api/operation/all/duplicate"),
                new EndpointInfo(HttpMethod.Get, "/")
            };

            var expectedMapped = new List<(EndpointInfo Endpoint, List<string> Methods)>
            {
                (new EndpointInfo(HttpMethod.Get, "/api/operation"), new List<string> { "GetNoPathMethod", "GetEmptyPathMethod" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/get"), new List<string> { "GetMethod" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/all"), new List<string> { "GetAllMethod" }),
                (new EndpointInfo(HttpMethod.Post, "/api/operation/all/duplicate"), new List<string> { "PostMethod", "PostDuplicateMethod" }),
                (new EndpointInfo(HttpMethod.Delete, "/api/operation/all"), new List<string> { "DeleteAllMethod" })
            };

            var jsonPath = Path.Combine("TestData", "coverageTestSwagger.json");
            var result = GetCoverageByClientFromFile(jsonPath, typeof(ITestController));

            result.NotMappedEndpoints.Should().BeEquivalentTo(expectedNotMapped);
            ValidateMappedEndpoints(result.MappedEndpoints, expectedMapped);
        }

        private static void ValidateMappedEndpoints(Dictionary<EndpointInfo, List<MethodInfo>> mappedEndpoints, List<(EndpointInfo Endpoint, List<string> Methods)> expected)
        {
            var actual = mappedEndpoints.Keys.Select(e => (e, ToStringList(mappedEndpoints[e]))).ToList();

            actual.Should().BeEquivalentTo(expected);
        }

        private static IList<string> ToStringList(List<MethodInfo> methods) => methods.Select(m => m.Name).ToList();
    }
}
