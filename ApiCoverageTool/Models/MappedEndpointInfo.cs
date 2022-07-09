using System.Net.Http;
using System.Reflection;

namespace ApiCoverageTool.Models;

public record MappedEndpointInfo(HttpMethod RestMethod, string Path, MethodBase MappedMethod)
    : EndpointInfo(RestMethod, Path);
