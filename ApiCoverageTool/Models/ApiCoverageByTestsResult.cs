using System.Collections.Generic;
using System.Reflection;

namespace ApiCoverageTool.Models
{
    public class ApiCoverageByTestsResult
    {
        public Dictionary<EndpointInfo, List<MethodInfo>> EndpointsCoveredByTests { get; init; } = new Dictionary<EndpointInfo, List<MethodInfo>>();
        public List<EndpointInfo> NotMappedEndpoints { get; init; } = new List<EndpointInfo>();
    }
}
