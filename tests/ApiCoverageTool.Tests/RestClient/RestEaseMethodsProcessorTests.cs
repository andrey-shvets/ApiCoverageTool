using System;
using System.Reflection;
using ApiCoverageTool.AssemblyUnderTests.Controllers;
using ApiCoverageTool.Extensions;
using ApiCoverageTool.RestClient;
using FluentAssertions;
using Xunit;

namespace ApiCoverageTool.Tests.RestClient
{
    public class RestEaseMethodsProcessorTests
    {
        private RestEaseMethodsProcessor RestProcessor { get; } = new RestEaseMethodsProcessor();

        #region IsRestMethod

        [Fact]
        public void IsRestMethod_GivenNull_ThrowsArgumentNullException()
        {
            MethodInfo method = null;
            Assert.Throws<ArgumentNullException>(() => RestProcessor.IsRestMethod(method));
        }

        [Theory]
        [InlineData("GetMethod")]
        [InlineData("PostMethod")]
        [InlineData("PutAllMethod")]
        [InlineData("PatchAllMethod")]
        [InlineData("DeleteAllMethod")]
        public void IsRestMethod_ForRestMethod_ReturnsFalse(string methodName)
        {
            var type = typeof(ITestController);
            var method = type.GetMethod(methodName);

            RestProcessor.IsRestMethod(method).Should().BeTrue();
        }

        #endregion IsRestMethod

        #region GetRestMethod

        [Fact]
        public void GetRestMethod_GivenNull_ThrowsArgumentNullException()
        {
            MethodInfo method = null;
            Assert.Throws<ArgumentNullException>(() => RestProcessor.GetRestMethod(method));
        }

        [Theory]
        [InlineData("GetMethod", "get")]
        [InlineData("PostMethod", "post")]
        [InlineData("PutAllMethod", "put")]
        [InlineData("PatchAllMethod", "patch")]
        [InlineData("DeleteAllMethod", "delete")]
        public void GetRestMethod_ForMethodConfiguredWithRestAttribute_ReturnsHttpMethod(string methodName, string expectedMethod)
        {
            var type = typeof(ITestController);
            var method = type.GetMethod(methodName);

            RestProcessor.GetRestMethod(method).Should().Be(expectedMethod.ToHttpMethod());
        }

        #endregion GetRestMethod

        #region GetFullPath

        [Fact]
        public void GetFullPath_GivenNull_ThrowsArgumentNullException()
        {
            MethodInfo method = null;
            Assert.Throws<ArgumentNullException>(() => RestProcessor.GetFullPath(method));
        }

        [Theory]
        [InlineData("GetNoPathMethod", "/api/operation")]
        [InlineData("GetEmptyPathMethod", "/api/operation")]
        [InlineData("GetMethod", "/api/operation/get")]
        [InlineData("PostMethod", "/api/operation/all/duplicate")]
        [InlineData("PostDuplicateMethod", "/api/operation/all/duplicate")]
        [InlineData("CallEndpointWithParameters", "/api/operation/withparameters")]
        public void GetFullPath_ForRestMethod_ReturnsFullEndpointPath(string methodName, string expectedPath)
        {
            var type = typeof(ITestController);
            var method = type.GetMethod(methodName);

            RestProcessor.GetFullPath(method).Should().Be(expectedPath);
        }

        [Theory]
        [InlineData("GetMethod", "/api/operation/get")]
        [InlineData("PostMethod", "/")]
        [InlineData("PostEmptyPathMethod", "/")]
        public void GetFullPath_ForRestMethodWithNoBasePath_ReturnsFullEndpointPath(string methodName, string expectedPath)
        {
            var type = typeof(ITestControllerNoBaseRout);
            var method = type.GetMethod(methodName);

            RestProcessor.GetFullPath(method).Should().Be(expectedPath);
        }

        #endregion GetFullPath
    }
}
