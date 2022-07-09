using System;
using System.Collections.Generic;
using System.Linq;
using ApiCoverageTool.Models;

namespace ApiCoverageTool.RestClient;

public static class RestClientAnalyzer<T> where T : IRestClientMethodsProcessor, new()
{
    public static IList<MappedEndpointInfo> GetRestMethodsFromClients(params Type[] controllers)
    {
        var mappedMethods = new List<MappedEndpointInfo>();

        foreach (var type in controllers.Distinct())
            mappedMethods.AddRange(GetRestMethodsFromClient(type));

        return mappedMethods;
    }

    public static IList<MappedEndpointInfo> GetRestMethodsFromClient(Type controller)
    {
        if (!controller.IsInterface)
            throw new ArgumentException($"{nameof(controller)} parameter is expected to be an interface.", nameof(controller));

        var methodsRetriever = new T();
        var mappedMethods = methodsRetriever.GetAllMappedEndpoints(controller);

        if (!mappedMethods.Any())
            throw new ArgumentException($"Provided interface has no methods mapped to endpoints.");

        return mappedMethods;
    }
}
