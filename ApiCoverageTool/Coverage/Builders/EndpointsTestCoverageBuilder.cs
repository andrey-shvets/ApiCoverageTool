using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApiCoverageTool.Models;
using ApiCoverageTool.RestClient;

namespace ApiCoverageTool.Coverage.Builders
{
    public class EndpointsTestCoverageBuilder<TProcessor> where TProcessor : IRestClientMethodsProcessor, new()
    {
        private IList<EndpointInfo> Endpoints { get; set; }
        private HashSet<Type> Controllers { get; set; } = new HashSet<Type>();
        private Assembly TestAssembly { get; set; }

        protected EndpointsTestCoverageBuilder()
        { }

        public static EndpointsTestCoverageBuilder<TProcessor> Instance() => new();

        public EndpointsTestCoverageBuilder<TProcessor> FromSwaggerJson(string swaggerJson)
        {
            if (swaggerJson is null)
                throw new ArgumentNullException(nameof(swaggerJson), "SwaggerJson can not be null.");

            Endpoints = SwaggerParser.ParseSwaggerApi(swaggerJson);

            return this;
        }

        public EndpointsTestCoverageBuilder<TProcessor> FromSwaggerJsonPath(string swaggerJsonPath)
        {
            if (string.IsNullOrWhiteSpace(swaggerJsonPath))
                throw new ArgumentException("Path to swaggerJson can not be null or empty.", nameof(swaggerJsonPath));

            Endpoints = SwaggerParser.ParseSwaggerApiFromFile(swaggerJsonPath);

            return this;
        }

        public EndpointsTestCoverageBuilder<TProcessor> ForController(Type controller)
        {
            if (controller is null)
                throw new ArgumentException("controller can not be null", nameof(controller));

            Controllers.Add(controller);

            return this;
        }

        public EndpointsTestCoverageBuilder<TProcessor> ForController<T>() where T : class => ForController(typeof(T));

        public EndpointsTestCoverageBuilder<TProcessor> WithTestsInAssembly(Assembly assembly)
        {
            if (assembly is null)
                throw new ArgumentException("Assembly can not be null", nameof(assembly));

            TestAssembly = assembly;

            return this;
        }

        public Dictionary<EndpointInfo, List<MethodInfo>> ControllerMethodsCoverage => ControllerMethodsTestCoverage<TProcessor>.GetTestCoverage(TestAssembly, Controllers.ToArray());
        public MappedApiResult MappingByController => ApiControllerMapping<TProcessor>.GetMappingByController(Endpoints, Controllers.ToArray());

        public MappedApiResult ApiTestCoverage
        {
            get
            {
                var apiCoverage = MappingByController;
                var mappedEndpoints = apiCoverage.EndpointsMapping.Keys;

                foreach (var endpoint in Endpoints)
                    if (!mappedEndpoints.Contains(endpoint))
                        apiCoverage.EndpointsMapping.Add(endpoint, new List<MethodInfo>());

                return apiCoverage;
            }
        }
    }
}
