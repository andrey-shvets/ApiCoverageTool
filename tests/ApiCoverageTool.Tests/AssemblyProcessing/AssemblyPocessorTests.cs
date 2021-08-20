using System;
using System.Collections.Generic;
using System.Reflection;
using ApiCoverageTool.AssemblyProcessing;
using ApiCoverageTool.Tests.ObjectsUnderTests;
using Xunit;
using static ApiCoverageTool.Tests.AssemblyProcessing.AssemblyPocessorTestsHelper;

namespace ApiCoverageTool.Tests
{
    public class AssemblyPocessorTests
    {
        private static Assembly AssemblyUnderTest => typeof(AssemblyUnderTests.MockClass).Assembly;

        #region GetAllTests
        [Fact]
        public void GetAllTests_WithNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => AssemblyPocessor.GetAllTests(null));
        }

        [Fact]
        public void GetAllTests_GivenAssemblyWithXUnitTests_ReturnsListOfTestMethods()
        {
            var expected = new List<string>
            {
                "MockFactAsync",
                "MockTheory",
                "NoClientCallTheoryAsync",
                "NoClientCallFact",
                "MockLambdaExpression",
                "MockLambdaExpressionAsync",
                "MockTestNestedCallAsync",
                "MockTestNestedCall",
                "MockTestDifferentClassStaticCall",
                "MockTestDifferentClassCall",
                "MockTestWithCycleInCallTree"
            };

            var tests = AssemblyPocessor.GetAllTests(AssemblyUnderTest);

            VerifyMethodsNames(tests, expected);
        }
        #endregion GetAllTests

        #region GetMethodCallsFromMethodBody
        [Fact]
        public void GetMethodCallsFromMethodBody_WithNull_ThrowsArgumentNullException()
        {
            MethodInfo method = null;
            Assert.Throws<ArgumentNullException>(() => AssemblyPocessor.GetMethodCallsFromMethodBody(method));
        }

        [Fact]
        public void GetMethodCallsFromMethodBody_GivenNonAsyncMethod_ReturnsListOfCallsFromProvidedMethod()
        {
            var type = typeof(AssemblyUnderTests.MockTests);
            var method = type.GetMethod("MockTheory");

            var methodsCalls = AssemblyPocessor.GetMethodCallsFromMethodBody(method);

            var expected = new List<string>
            {
                "GetMethod",
                "NotTestMethodNoClientCall"
            };

            VerifyMethodsNames(methodsCalls, expected);
        }

        [Fact]
        public void GetMethodCallsFromMethodBody_GivenAsyncMethod_ReturnsListOfCallsFromProvidedMethod()
        {
            var type = typeof(AssemblyUnderTests.MockTests);
            var method = type.GetMethod("MockFactAsync");

            var methodsCalls = AssemblyPocessor.GetMethodCallsFromMethodBody(method, new[] { "ApiCoverageTool.AssemblyUnderTests" });

            var expected = new List<string>
            {
                "NotTestMethodAsync",
                "GetMethod",
                "NotTestMethodNoClientCall"
            };

            VerifyMethodsNames(methodsCalls, expected);
        }
        #endregion GetMethodCallsFromMethodBody    

        #region GetAllMethodCalls
        [Fact]
        public void GetAllMethodCalls_WithNull_ThrowsArgumentNullException()
        {
            MethodInfo method = null;
            Assert.Throws<ArgumentNullException>(() => AssemblyPocessor.GetAllMethodCalls(method));
        }

        [Fact]
        public void GetAllMethodCalls_GivenNonAsyncMethod_ReturnsListOfCallsFromProvidedMethod()
        {
            var type = typeof(AssemblyUnderTests.MoreMockTests);
            var method = type.GetMethod("MockTestNestedCall");

            var methodsCalls = AssemblyPocessor.GetAllMethodCalls(method);

            var expected = new List<string>
            {
                "NotTestMethod",
                "NotTestMethodNoClientCall",
                "PatchAllMethod",
                "GetMethod"
            };

            VerifyMethodsNames(methodsCalls, expected);
        }

        [Fact]
        public void GetAllMethodCalls_WithListOfAllowedAssemblies_ReturnsListOfCallsFromProvidedMethodInTheseAssemblies()
        {
            var type = typeof(TestClass);
            var method = type.GetMethod("AbstractMethod");

            var methodsCalls = AssemblyPocessor.GetAllMethodCalls(method, new[] { typeof(AssemblyUnderTests.MockClass).Assembly.GetName().Name });

            var expected = new List<string>
            {
                "NotTestMethodNoClientCall"
            };

            VerifyMethodsNames(methodsCalls, expected);
        }

        [Fact]
        public void GetAllMethodCalls_GivenAsyncMethod_ReturnsListOfCallsFromProvidedMethod()
        {
            var type = typeof(AssemblyUnderTests.MoreMockTests);
            var method = type.GetMethod("MockTestNestedCallAsync");

            var methodsCalls = AssemblyPocessor.GetAllMethodCalls(method);

            var expected = new List<string>
            {
                "GetNoPathMethod",
                "NonRestMethod",
                "NotTestMethodAsync",
                "GetMethod"
            };

            VerifyMethodsNames(methodsCalls, expected);
        }

        [Fact]
        public void GetAllMethodCalls_GiveMethodThatHasCycleInCAllTree_ReturnsListOfCallsFromProvidedMethod()
        {
            var type = typeof(AssemblyUnderTests.MoreMockTests);
            var method = type.GetMethod("MockTestWithCycleInCallTree");

            var methodsCalls = AssemblyPocessor.GetAllMethodCalls(method);

            var expected = new List<string>
            {
                "MethodWithRecursion",
                "MethodWithTheCycle",
                "NotTestMethod",
                "MethodWithTheCycle2",
                "GetMethod",
                "MethodWithTheCycle3"
            };

            VerifyMethodsNames(methodsCalls, expected);
        }
        #endregion GetAllMethodCalls      
    }
}
