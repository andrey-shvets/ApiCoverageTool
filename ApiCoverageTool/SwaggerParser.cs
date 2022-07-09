using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ApiCoverageTool.Exceptions;
using ApiCoverageTool.Extensions;
using ApiCoverageTool.Models;

namespace ApiCoverageTool;

public static class SwaggerParser
{
    public static async Task<IList<EndpointInfo>> ParseSwaggerApiFromUri(Uri swaggerJsonUri)
    {
        var client = new HttpClient();
        var json = await client.GetStringAsync(swaggerJsonUri);

        return ParseSwaggerApi(json);
    }

    public static IList<EndpointInfo> ParseSwaggerApiFromFile(string swaggerJsonPath)
    {
        var json = File.ReadAllText(swaggerJsonPath);

        return ParseSwaggerApi(json);
    }

    public static IList<EndpointInfo> ParseSwaggerApi(string swaggerJson)
    {
        swaggerJson.IsNotNullValidation(nameof(swaggerJson));

        try
        {
            var swaggerEndpoints = RetrieveMSwaggerModelFromJson(swaggerJson);
            return GetEndpointsList(swaggerEndpoints).ToList();
        }
        catch (Exception ex) when (ex is not InvalidSwaggerJsonException)
        {
            throw new InvalidSwaggerJsonException($"{nameof(swaggerJson)} parameter contains invalid swagger json.", swaggerJson, ex);
        }
    }

    private static SwaggerModel RetrieveMSwaggerModelFromJson(string swaggerJson)
    {
        var model = JsonSerializer.Deserialize<SwaggerModel>(swaggerJson);

        if (!model.Paths.Any())
            throw new InvalidSwaggerJsonException($"{nameof(swaggerJson)} doesn't have any endpoints.", swaggerJson);

        if (model.Paths.Values.Any(p => p.Count == 0))
            throw new InvalidSwaggerJsonException($"{nameof(swaggerJson)} has endpoints with no operations.", swaggerJson);

        return model;
    }

    private static IEnumerable<EndpointInfo> GetEndpointsList(SwaggerModel swaggerEndpoints)
    {
        foreach (var path in swaggerEndpoints.Paths.Keys)
        {
            var methods = swaggerEndpoints.Paths[path].Keys;

            if (path == "/")
                continue;

            var fixedPath = $"/{path.Trim('/').ToLower()}";

            foreach (var method in methods)
                yield return new EndpointInfo(method.ToHttpMethod(), fixedPath);
        }
    }
}
