﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ApiCoverageTool.Models;

public class ApiCoverageResult
{
    public Dictionary<EndpointInfo, List<MethodBase>> EndpointsMapping { get; init; } = new Dictionary<EndpointInfo, List<MethodBase>>();
    public Dictionary<EndpointInfo, List<MethodBase>> CoveredEndpoints => EndpointsMapping.Where(e => e.Value.Any()).ToDictionary(e => e.Key, e => e.Value);
    public List<EndpointInfo> NotCoveredEndpoints => EndpointsMapping.Where(e => e.Value is null || !e.Value.Any()).Select(e => e.Key).ToList();
}
