using System;
using System.Collections.Generic;
using System.Net.Http;
using ApiCoverageTool.Extensions;
using ApiCoverageTool.Models;
using FluentAssertions;
using Xunit;

namespace ApiCoverageTool.Tests
{
    public class SwaggerApiExtentionTests
    {
        [Fact]
        public void ToMethodPathList_OnEmptyEndpointsList_ReturnsEmptyList()
        {
            var endpoints = new List<EndpointInfo>();

            endpoints.ToMethodPathList().Should().BeEmpty();
        }

        [Fact]
        public void ToMethodPathList_OnListOfEndpoints_ReturnsListOfMethodWithPathStrings()
        {
            var endpoints = new List<EndpointInfo>
            {
                new EndpointInfo(HttpMethod.Get, "/"),
                new EndpointInfo(HttpMethod.Get, "/api/get"),
                new EndpointInfo(HttpMethod.Post, "/api/post"),
            };

            var expected = new List<string>
            {
                "GET /",
                "GET /api/get",
                "POST /api/post",
            };

            endpoints.ToMethodPathList().Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToHttpMethod_WithValidHttpMethodName_ReturnsHttpMethod()
        {
            "get".ToHttpMethod().Should().Be(HttpMethod.Get);
            "POST".ToHttpMethod().Should().Be(HttpMethod.Post);
            "Put".ToHttpMethod().Should().Be(HttpMethod.Put);
            "PAtch".ToHttpMethod().Should().Be(HttpMethod.Patch);
            "DeletE".ToHttpMethod().Should().Be(HttpMethod.Delete);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("unknown")]
        [InlineData("getmethod")]
        public void ToHttpMethod_WithInvalidHttpMethodName_ThrowsArgumentException(string methodName)
        {
            Assert.Throws<ArgumentException>(() => methodName.ToHttpMethod());
        }
    }
}
