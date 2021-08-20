using System;
using System.Collections.Generic;
using System.Net.Http;
using ApiCoverageTool.Models;

namespace ApiCoverageTool.Extentions
{
    public static class SwaggerApiExtentions
    {
        public static IEnumerable<string> ToMethodPathList(this IEnumerable<EndpointInfo> swaggerEndpoints)
        {
            foreach (var endpointInfo in swaggerEndpoints)
                    yield return endpointInfo.ToString();
        }

        public static HttpMethod ToHttpMethod(this string httpMethodName) => httpMethodName?.ToLower() switch
        {
            "get" => HttpMethod.Get,
            "post" => HttpMethod.Post,
            "put" => HttpMethod.Put,
            "patch" => HttpMethod.Patch,
            "delete" => HttpMethod.Delete,
            _ => throw new ArgumentException(nameof(httpMethodName), $"{httpMethodName} is invalid HttpMethod"),
        };
    }
}
