using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using ApiCoverageTool.Extensions;
using ApiCoverageTool.Models;
using RestEase;

namespace ApiCoverageTool.RestClient;

public class RestEaseMethodsProcessor : IRestClientMethodsProcessor
{
    public bool IsRestMethod(MethodInfo method) => GetRestMethod(method) is not null;

    public HttpMethod GetRestMethod(MethodInfo method)
    {
        method.IsNotNullValidation(nameof(method));

        return method.GetCustomAttribute<RequestAttributeBase>()?.Method;
    }

    public string GetFullPath(MethodInfo method)
    {
        method.IsNotNullValidation(nameof(method));

        if (!IsRestMethod(method))
            throw new ArgumentException($"Failed to retrieve full endpoint path for method {method.Name}", nameof(method));

        var basePath = method.DeclaringType.GetCustomAttribute<BasePathAttribute>()?.BasePath?.Trim('/').ToLower();
        var path = method.GetCustomAttribute<RequestAttributeBase>()?.Path?.Trim('/').ToLower();

        if (path is not null)
            path = Regex.Replace(path, @"\?.*", string.Empty);

        if (string.IsNullOrEmpty(basePath) && string.IsNullOrEmpty(path))
            return "/";

        if (string.IsNullOrEmpty(path))
            return $"/{basePath}";

        if (string.IsNullOrEmpty(basePath))
            return $"/{path}";

        return $"/{basePath}/{path}";
    }

    public IList<MappedEndpointInfo> GetAllMappedEndpoints(Type controller)
    {
        var mappedGetMethods = RetrieveMappedRestMethods<GetAttribute>(controller, HttpMethod.Get);
        var mappedPostMethods = RetrieveMappedRestMethods<PostAttribute>(controller, HttpMethod.Post);
        var mappedPutMethods = RetrieveMappedRestMethods<PutAttribute>(controller, HttpMethod.Put);
        var mappedPatchMethods = RetrieveMappedRestMethods<PatchAttribute>(controller, HttpMethod.Patch);
        var mappedDeleteMethods = RetrieveMappedRestMethods<DeleteAttribute>(controller, HttpMethod.Delete);

        var mappedMethods = mappedGetMethods.ToList();
        mappedMethods.AddRange(mappedPostMethods);
        mappedMethods.AddRange(mappedPutMethods);
        mappedMethods.AddRange(mappedPatchMethods);
        mappedMethods.AddRange(mappedDeleteMethods);

        return mappedMethods;
    }

    private IEnumerable<MappedEndpointInfo> RetrieveMappedRestMethods<T>(Type controller, HttpMethod httpMethod)
        where T : RequestAttributeBase
    {
        var mappedMethods = controller.GetMethods().Where(m => m.GetCustomAttributes<T>().Any()).ToList();

        foreach (var method in mappedMethods)
            yield return new MappedEndpointInfo(httpMethod, GetFullPath(method), method);
    }
}
