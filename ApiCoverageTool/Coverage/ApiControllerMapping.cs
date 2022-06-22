using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiCoverageTool.Extensions;
using ApiCoverageTool.Models;
using ApiCoverageTool.RestClient;

namespace ApiCoverageTool.Coverage;

public static class ApiControllerMapping<T> where T : IRestClientMethodsProcessor, new()
{
    public static async Task<ApiCoverageResult> GetMappingByControllerFromUri(Uri swaggerJsonUri, params Type[] controllers)
    {
        var serviceEndpoints = await SwaggerParser.ParseSwaggerApiFromUri(swaggerJsonUri);

        return GetMappingByController(serviceEndpoints, controllers);
    }

    public static async Task<ApiCoverageResult> GetMappingByControllerFromUri(string swaggerJsonUri, params Type[] controllers)
    {
        var uri = new Uri(swaggerJsonUri);

        return await GetMappingByControllerFromUri(uri, controllers);
    }

    public static ApiCoverageResult GetMappingByControllerFromFile(string swaggerJsonPath, params Type[] controllers)
    {
        var serviceEndpoints = SwaggerParser.ParseSwaggerApiFromFile(swaggerJsonPath);

        return GetMappingByController(serviceEndpoints, controllers);
    }

    public static ApiCoverageResult GetMappingByController(string swaggerJson, params Type[] controllers)
    {
        var serviceEndpoints = SwaggerParser.ParseSwaggerApi(swaggerJson);

        return GetMappingByController(serviceEndpoints, controllers);
    }

    public static ApiCoverageResult GetMappingByController(IList<EndpointInfo> serviceEndpoints,
        params Type[] controllers)
    {
        var mappedEndpoints = RestClientAnalyzer<T>.GetRestMethodsFromClients(controllers);

        serviceEndpoints.IsNotNullValidation(nameof(serviceEndpoints));
        mappedEndpoints.IsNotNullValidation(nameof(mappedEndpoints));

        var result = new ApiCoverageResult();

        foreach (var endpoint in serviceEndpoints)
        {
            var mappedMethods = mappedEndpoints.Where(m => m.RestMethod == endpoint.RestMethod && string.Equals(m.Path, endpoint.Path, StringComparison.InvariantCultureIgnoreCase)).Select(m => m.MappedMethod).ToList();

            result.EndpointsMapping[endpoint] = mappedMethods;
        }

        return result;
    }
}
