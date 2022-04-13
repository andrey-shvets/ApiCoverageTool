using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using ApiCoverageTool.Models;
using ApiCoverageTool.RestClient;

namespace ApiCoverageTool.Coverage.Builders
{
    public class EndpointsTestCoverageBuilder<TProcessor> where TProcessor : IRestClientMethodsProcessor, new()
    {
        private IList<EndpointInfo> Endpoints { get; set; }
        private HashSet<Type> Controllers { get; } = new HashSet<Type>();
        private Assembly TestAssembly { get; init; }

        protected EndpointsTestCoverageBuilder()
        { }

        public static EndpointsTestCoverageBuilder<TProcessor> ForTestsInAssembly(Assembly assembly)
        {
            if (assembly is null)
                throw new ArgumentException("Assembly can not be null", nameof(assembly));

            var builder = new EndpointsTestCoverageBuilder<TProcessor>
            {
                TestAssembly = assembly
            };

            return builder;
        }

        public EndpointsTestCoverageBuilder<TProcessor> UseSwagger(HttpClient client)
        {
            if (client is null)
                throw new ArgumentNullException(nameof(client), "Http client can not be null.");

            var swaggerJsonUri = "swagger/v1/swagger.json";
            var swaggerJson = client.GetStringAsync(swaggerJsonUri).Result;

            return UseSwaggerJson(swaggerJson);
        }

        public EndpointsTestCoverageBuilder<TProcessor> UseSwaggerUrl(string swaggerUrl)
        {
            if (swaggerUrl is null)
                throw new ArgumentNullException(nameof(swaggerUrl), "Swagger url can not be null.");

            if (!swaggerUrl.EndsWith("/swagger.json"))
                throw new ArgumentException(nameof(swaggerUrl), $"Invalid swagger url, should be in format 'https://.../swagger.json'. Provided url: {swaggerUrl}");

            var swaggerJson = new HttpClient().GetStringAsync(swaggerUrl).Result;

            return UseSwaggerJson(swaggerJson);
        }

        public EndpointsTestCoverageBuilder<TProcessor> UseSwaggerJson(string swaggerJson)
        {
            if (swaggerJson is null)
                throw new ArgumentNullException(nameof(swaggerJson), "SwaggerJson can not be null.");

            Endpoints = SwaggerParser.ParseSwaggerApi(swaggerJson);

            return this;
        }

        public EndpointsTestCoverageBuilder<TProcessor> UseSwaggerJsonPath(string swaggerJsonPath)
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

        public EndpointsTestCoverageBuilder<TProcessor> ForController<TController>() where TController : class => ForController(typeof(TController));

        public Dictionary<EndpointInfo, List<MethodInfo>> ControllerMethodsCoverage => ControllerMethodsTestCoverage<TProcessor>.GetTestCoverage(TestAssembly, Controllers.ToArray());
        public MappedApiResult MappingByController => ApiControllerMapping<TProcessor>.GetMappingByController(Endpoints, Controllers.ToArray());

        public MappedApiResult ApiTestCoverage
        {
            get
            {
                if (Endpoints is null)
                    throw new InvalidOperationException("Looks like swagger source was not specified. Use UseSwagger, UseSwaggerJson or UseSwaggerJsonPath methods.");

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
