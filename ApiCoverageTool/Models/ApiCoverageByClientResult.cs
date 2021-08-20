using System.Collections.Generic;
using System.Reflection;

namespace ApiCoverageTool.Models
{
    public class ApiCoverageByClientResult
    {
        public Dictionary<EndpointInfo, List<MethodInfo>> MappedEndpoints { get; init; } = new Dictionary<EndpointInfo, List<MethodInfo>>();
        public List<EndpointInfo> NotMappedEndpoints { get; init; } = new List<EndpointInfo>();
    }
}
