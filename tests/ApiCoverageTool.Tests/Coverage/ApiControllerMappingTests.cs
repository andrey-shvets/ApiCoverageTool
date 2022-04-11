﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using ApiCoverageTool.AssemblyUnderTests.Controllers;
using ApiCoverageTool.Models;
using ApiCoverageTool.Tests.Helpers;
using FluentAssertions;
using Xunit;
using static ApiCoverageTool.Coverage.ApiControllerMapping<ApiCoverageTool.RestClient.RestEaseMethodsProcessor>;

namespace ApiCoverageTool.Tests.Coverage
{
    public class ApiControllerMappingTests
    {
        [Fact]
        public void GetMappingByController_WithNullServiceEndpoints_ThrowsArgumentException() =>
            Assert.Throws<ArgumentNullException>(() => GetMappingByController((IList<EndpointInfo>)null, Array.Empty<Type>()));

        [Fact]
        public void GetMappingByController_WithNullMappedEndpoints_ThrowsArgumentException() =>
            Assert.Throws<ArgumentNullException>(() => GetMappingByController(new List<EndpointInfo>(), null));

        [Fact]
        public void GetMappingByControllerFromFile_WithSwaggerJson_ReturnsListOfOperationsAndEndpoints()
        {
            var expectedNotCoveredEndpoints = new List<EndpointInfo>()
            {
                new EndpointInfo(HttpMethod.Post, "/api/operation/all"),
                new EndpointInfo(HttpMethod.Get, "/api/operation/all/duplicate"),
                new EndpointInfo(HttpMethod.Get, "/")
            };

            var expectedCoveredEndpoints = new List<(EndpointInfo Endpoint, List<string> Methods)>
            {
                (new EndpointInfo(HttpMethod.Get, "/api/operation"), new List<string> { "GetNoPathMethod", "GetEmptyPathMethod" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/get"), new List<string> { "GetMethod" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/all"), new List<string> { "GetAllMethod" }),
                (new EndpointInfo(HttpMethod.Post, "/api/operation/all/duplicate"), new List<string> { "PostMethod", "PostDuplicateMethod" }),
                (new EndpointInfo(HttpMethod.Delete, "/api/operation/all"), new List<string> { "DeleteAllMethod" })
            };

            var expectedEndpoints = new List<(EndpointInfo Endpoint, List<string> Methods)>
            {
                (new EndpointInfo(HttpMethod.Get, "/api/operation"), new List<string> { "GetNoPathMethod", "GetEmptyPathMethod" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/get"), new List<string> { "GetMethod" }),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/all"), new List<string> { "GetAllMethod" }),
                (new EndpointInfo(HttpMethod.Post, "/api/operation/all/duplicate"), new List<string> { "PostMethod", "PostDuplicateMethod" }),
                (new EndpointInfo(HttpMethod.Delete, "/api/operation/all"), new List<string> { "DeleteAllMethod" }),
                (new EndpointInfo(HttpMethod.Post, "/api/operation/all"), new List<string>()),
                (new EndpointInfo(HttpMethod.Get, "/api/operation/all/duplicate"), new List<string>()),
                (new EndpointInfo(HttpMethod.Get, "/"), new List<string>())
            };

            var jsonPath = Path.Combine("TestData", "coverageTestSwagger.json");
            var result = GetMappingByControllerFromFile(jsonPath, typeof(ITestController));

            result.NotCoveredEndpoints.Should().BeEquivalentTo(expectedNotCoveredEndpoints);
            result.CoveredEndpoints.ValidateMappedEndpoints(expectedCoveredEndpoints);
            result.EndpointsMapping.ValidateMappedEndpoints(expectedEndpoints);
        }
    }
}
