using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApiCoverageTool.AssemblyProcessing;
using ApiCoverageTool.Extensions;
using ApiCoverageTool.Models;
using ApiCoverageTool.RestClient;

namespace ApiCoverageTool.Coverage;

public static class ControllerMethodsTestCoverage<T> where T : IRestClientMethodsProcessor, new()
{
    public static Dictionary<EndpointInfo, List<MethodInfo>> GetTestCoverage(Assembly testsAssembly, params Type[] controllers)
    {
        testsAssembly.IsNotNullValidation(nameof(testsAssembly));
        controllers.IsNotNullValidation(nameof(controllers));

        var allTests = AssemblyProcessor.GetAllTests(testsAssembly);

        return GetTestCoverage(allTests, controllers);
    }

    private static Dictionary<EndpointInfo, List<MethodInfo>> GetTestCoverage(IEnumerable<MethodInfo> allTests, Type[] controllers)
    {
        var result = new Dictionary<EndpointInfo, List<MethodInfo>>();

        foreach (var test in allTests)
        {
            var endpointsCalled = GetAllEndpointsCalledFromMethod(test, controllers);

            foreach (var endpoints in endpointsCalled)
            {
                if (result.ContainsKey(endpoints))
                    result[endpoints].Add(test);
                else
                    result[endpoints] = new List<MethodInfo> { test };
            }
        }

        return result;
    }

    private static IList<EndpointInfo> GetAllEndpointsCalledFromMethod(MethodInfo method, Type[] controllers)
    {
        var restProcessor = new T();
        var restMethodsCalled = AssemblyProcessor.GetAllMethodCalls(method)
            .Where(m => restProcessor.IsRestMethod(m))
            .Where(m => controllers.Contains(m.DeclaringType))
            .ToList();

        var endpoints = restMethodsCalled
            .Select(m => new EndpointInfo(restProcessor.GetRestMethod(m), restProcessor.GetFullPath(m)))
            .Distinct().ToList();

        return endpoints;
    }
}
