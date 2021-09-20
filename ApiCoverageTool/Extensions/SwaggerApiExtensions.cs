using System;
using System.Collections.Generic;
using System.Net.Http;
using ApiCoverageTool.Models;

namespace ApiCoverageTool.Extensions
{
    public static class SwaggerApiExtensions
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
            _ => throw new ArgumentException($"{httpMethodName} is invalid HttpMethod", nameof(httpMethodName)),
        };
    }
}
