using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using ApiCoverageTool.Models;

namespace ApiCoverageTool.RestClient;

public interface IRestClientMethodsProcessor
{
    public bool IsRestMethod(MethodInfo method);

    public HttpMethod GetRestMethod(MethodInfo method);

    public string GetFullPath(MethodInfo method);

    public IList<MappedEndpointInfo> GetAllMappedEndpoints(Type controller);
}
