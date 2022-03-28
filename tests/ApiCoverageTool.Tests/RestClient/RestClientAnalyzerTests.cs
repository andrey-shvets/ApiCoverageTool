using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using ApiCoverageTool.AssemblyUnderTests.Controllers;
using ApiCoverageTool.Models;
using ApiCoverageTool.Tests.ObjectsUnderTests;
using FluentAssertions;
using Xunit;
using static ApiCoverageTool.RestClient.RestClientAnalyzer<ApiCoverageTool.RestClient.RestEaseMethodsProcessor>;

namespace ApiCoverageTool.Tests.RestClient
{
    public class RestClientAnalyzerTests
    {
        #region GetRestMethodsFromClient

        [Fact]
        public void GetRestMethodsFromClient_GivenClass_ThrowsArgumentException() => Assert.Throws<ArgumentException>(() => GetRestMethodsFromClient(typeof(TestClientClass)));

        [Fact]
        public void GetRestMethodsFromClient_GivenControllerWithNoMappedServiceMethods_ThrowsArgumentException() => Assert.Throws<ArgumentException>(() => GetRestMethodsFromClient(typeof(ITestControllerNoMethods)));

        [Fact]
        public void GetRestMethodsFromClient_GivenControllerInterface_ReturnsListOfOperationsAndEndpoints()
        {
            var expectedEndpoints = new List<(HttpMethod Method, string Path, string MappedMethodName)>
            {
                (HttpMethod.Get, "/api/operation", "GetNoPathMethod"),
                (HttpMethod.Get, "/api/operation", "GetEmptyPathMethod"),
                (HttpMethod.Get, "/api/operation/get", "GetMethod"),
                (HttpMethod.Get, "/api/operation/all", "GetAllMethod"),
                (HttpMethod.Post, "/api/operation/all/duplicate", "PostMethod"),
                (HttpMethod.Post, "/api/operation/all/duplicate", "PostDuplicateMethod"),
                (HttpMethod.Put, "/api/operation/all", "PutAllMethod"),
                (HttpMethod.Patch, "/api/operation/all", "PatchAllMethod"),
                (HttpMethod.Delete, "/api/operation/all", "DeleteAllMethod")
            };

            var endpoints = GetRestMethodsFromClient(typeof(ITestController));

            ValidateMappedEndpoints(endpoints, expectedEndpoints);
        }

        [Fact]
        public void GetRestMethodsFromClient_GivenControllerInterfaceWithNoBaseRout_ReturnsListOfOperationsAndEndpoints()
        {
            var expectedEndpoints = new List<(HttpMethod Method, string Path, string MappedMethodName)>
            {
                (HttpMethod.Get, "/api/operation/get", "GetMethod"),
                (HttpMethod.Post, "/", "PostMethod"),
                (HttpMethod.Post, "/", "PostEmptyPathMethod")
            };

            var endpoints = GetRestMethodsFromClient(typeof(ITestControllerNoBaseRout));

            ValidateMappedEndpoints(endpoints, expectedEndpoints);
        }

        #endregion GetRestMethodsFromClient

        #region GetRestMethodsFromClients

        [Fact]
        public void GetRestMethodsFromClients_GivenClass_ThrowsArgumentException() => Assert.Throws<ArgumentException>(() => GetRestMethodsFromClients(typeof(ITestController), typeof(TestClientClass)));

        [Fact]
        public void GetRestMethodsFromClients_GivenControllerWithNoMappedServiceMethods_ThrowsArgumentException() => Assert.Throws<ArgumentException>(() => GetRestMethodsFromClients(typeof(ITestControllerNoMethods), typeof(ITestController)));

        [Fact]
        public void GetRestMethodsFromClients_GivenControllerInterfaces_ReturnsListOfOperationsAndEndpoints()
        {
            var expectedEndpoints = new List<(HttpMethod Method, string Path, string MappedMethodName)>
            {
                (HttpMethod.Get, "/api/operation", "GetNoPathMethod"),
                (HttpMethod.Get, "/api/operation", "GetEmptyPathMethod"),
                (HttpMethod.Get, "/api/operation/get", "GetMethod"),
                (HttpMethod.Get, "/api/operation/all", "GetAllMethod"),
                (HttpMethod.Post, "/api/operation/all/duplicate", "PostMethod"),
                (HttpMethod.Post, "/api/operation/all/duplicate", "PostDuplicateMethod"),
                (HttpMethod.Put, "/api/operation/all", "PutAllMethod"),
                (HttpMethod.Patch, "/api/operation/all", "PatchAllMethod"),
                (HttpMethod.Delete, "/api/operation/all", "DeleteAllMethod"),
                (HttpMethod.Get, "/api/operation/get", "GetMethod"),
                (HttpMethod.Post, "/", "PostMethod"),
                (HttpMethod.Post, "/", "PostEmptyPathMethod")
            };

            var endpoints = GetRestMethodsFromClients(typeof(ITestController), typeof(ITestControllerNoBaseRout));

            ValidateMappedEndpoints(endpoints, expectedEndpoints);
        }

        [Fact]
        public void GetRestMethodsFromClients_GivenDuplicatedTypes_ReturnsListOfOperationsAndEndpointsWithoutDuplicates()
        {
            var expectedEndpoints = new List<(HttpMethod Method, string Path, string MappedMethodName)>
            {
                (HttpMethod.Get, "/api/operation/get", "GetMethod"),
                (HttpMethod.Post, "/", "PostMethod"),
                (HttpMethod.Post, "/", "PostEmptyPathMethod")
            };

            var endpoints = GetRestMethodsFromClients(typeof(ITestControllerNoBaseRout), typeof(ITestControllerNoBaseRout));

            ValidateMappedEndpoints(endpoints, expectedEndpoints);
        }

        #endregion GetRestMethodsFromClients

        private static void ValidateMappedEndpoints(IEnumerable<MappedEndpointInfo> mappedEndpoints, IEnumerable<(HttpMethod Method, string Path, string MappedMethodName)> expected)
        {
            var actual = mappedEndpoints.Select(e => (e.RestMethod, e.Path, e.MappedMethod.Name));

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
