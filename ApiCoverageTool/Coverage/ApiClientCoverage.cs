using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCoverageTool.Extensions;
using ApiCoverageTool.Models;
using ApiCoverageTool.RestClient;

namespace ApiCoverageTool.Coverage
{
    public static class ApiClientCoverage<T> where T : IRestClientMethodsProcessor, new()
    {
        public static async Task<ApiCoverageByClientResult> GetCoverageByClientFromUri(Uri swaggerJsonUri, params Type[] clientTypes)
        {
            var serviceEndpoints = await SwaggerParser.ParseSwaggerApiFromUri(swaggerJsonUri);
            var mappedEndpoints = RestClientAnalyzer<T>.GetRestMethodsFromClients(clientTypes);

            return GetCoverageByClient(serviceEndpoints, mappedEndpoints);
        }

        public static async Task<ApiCoverageByClientResult> GetCoverageByClientFromUri(string swaggerJsonUriString, params Type[] clientTypes)
        {
            var swaggerJsonUri = new Uri(swaggerJsonUriString);
            return await GetCoverageByClientFromUri(swaggerJsonUri, clientTypes);
        }

        public static ApiCoverageByClientResult GetCoverageByClientFromFile(string swaggerJsonPath, params Type[] clientTypes)
        {
            var serviceEndpoints = SwaggerParser.ParseSwaggerApiFromFile(swaggerJsonPath);
            var mappedEndpoints = RestClientAnalyzer<T>.GetRestMethodsFromClients(clientTypes);

            return GetCoverageByClient(serviceEndpoints, mappedEndpoints);
        }

        public static ApiCoverageByClientResult GetCoverageByClient(string swaggerJson, params Type[] clientTypes)
        {
            var serviceEndpoints = SwaggerParser.ParseSwaggerApi(swaggerJson);
            var mappedEndpoints = clientTypes.SelectMany(c => RestClientAnalyzer<T>.GetRestMethodsFromClients(c)).ToList();

            return GetCoverageByClient(serviceEndpoints, mappedEndpoints);
        }

        public static ApiCoverageByClientResult GetCoverageByClient(IList<EndpointInfo> serviceEndpoints,
            IList<MappedEndpointInfo> mappedEndpoints)
        {
            serviceEndpoints.IsNotNullValidation(nameof(serviceEndpoints));
            mappedEndpoints.IsNotNullValidation(nameof(mappedEndpoints));

            var result = new ApiCoverageByClientResult();

            foreach (var endpoint in serviceEndpoints)
            {
                var mappedMethods = mappedEndpoints.Where(m => m.RestMethod == endpoint.RestMethod && string.Equals(m.Path, endpoint.Path, StringComparison.InvariantCultureIgnoreCase)).Select(m => m.MappedMethod);

                if (mappedMethods.Any())
                    result.MappedEndpoints[endpoint] = mappedMethods.ToList();
                else
                    result.NotMappedEndpoints.Add(endpoint);
            }

            return result;
        }
    }
}
