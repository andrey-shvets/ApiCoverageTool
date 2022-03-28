using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApiCoverageTool.AssemblyProcessing;
using ApiCoverageTool.Extensions;
using ApiCoverageTool.Models;
using ApiCoverageTool.RestClient;

namespace ApiCoverageTool.Coverage
{
    public static class ApiTestCoverage<T> where T : IRestClientMethodsProcessor, new()
    {
        public static Dictionary<EndpointInfo, List<MethodInfo>> GetTestCoverage(Assembly testsAssembly, params Type[] clientTypes)
        {
            testsAssembly.IsNotNullValidation(nameof(testsAssembly));
            clientTypes.IsNotNullValidation(nameof(clientTypes));

            var allTests = AssemblyProcessor.GetAllTests(testsAssembly);

            return GetTestCoverage(allTests, clientTypes);
        }

        private static Dictionary<EndpointInfo, List<MethodInfo>> GetTestCoverage(IEnumerable<MethodInfo> allTests, Type[] clientTypes)
        {
            var result = new Dictionary<EndpointInfo, List<MethodInfo>>();

            foreach (var test in allTests)
            {
                var endpointsCalled = GetAllEndpointsCalledFromMethod(test, clientTypes);

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

        private static IList<EndpointInfo> GetAllEndpointsCalledFromMethod(MethodInfo method, Type[] clientTypes)
        {
            var restProcessor = new T();
            var restMethodsCalled = AssemblyProcessor.GetAllMethodCalls(method)
                .Where(m => restProcessor.IsRestMethod(m))
                .Where(m => clientTypes.Contains(m.DeclaringType))
                .ToList();

            var endpoints = restMethodsCalled
                .Select(m => new EndpointInfo(restProcessor.GetRestMethod(m), restProcessor.GetFullPath(m)))
                .Distinct().ToList();

            return endpoints;
        }
    }
}
