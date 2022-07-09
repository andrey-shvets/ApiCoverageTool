using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using ApiCoverageTool.Models;

namespace ApiCoverageTool.RestClient;

public interface IRestClientMethodsProcessor
{
    public bool IsRestMethod(MethodBase method);

    public HttpMethod GetRestMethod(MethodBase method);

    public string GetFullPath(MethodBase method);

    public IList<MappedEndpointInfo> GetAllMappedEndpoints(Type controller);
}
