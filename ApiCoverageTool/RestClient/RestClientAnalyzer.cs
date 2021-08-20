using System;
using System.Collections.Generic;
using System.Linq;
using ApiCoverageTool.Models;

namespace ApiCoverageTool.RestClient
{
    public static class RestClientAnalyzer<T> where T : IRestClientMethodsProcessor, new()
    {
        public static IList<MappedEndpointInfo> GetRestMethodsFromClients(params Type[] clientsTypes)
        {
            var mappedMethods = new List<MappedEndpointInfo>();

            foreach (var type in clientsTypes.Distinct())
                mappedMethods.AddRange(GetRestMethodsFromClient(type));

            return mappedMethods;
        }

        public static IList<MappedEndpointInfo> GetRestMethodsFromClient(Type clientType)
        {
            if (!clientType.IsInterface)
                throw new ArgumentException(nameof(clientType),
                    $"{nameof(clientType)} parameter is expected to be an interface.");

            var methodsRetriver = new T();
            var mappedMethods = methodsRetriver.GetAllMappedEndpoints(clientType);

            if (!mappedMethods.Any())
                throw new ArgumentException($"Provided interface has no methods mapped to endpoints.");

            return mappedMethods;
        }
    }
}
