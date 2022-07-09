using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApiCoverageTool.Models;
using FluentAssertions;

namespace ApiCoverageTool.Tests.Helpers;

public static class ValidationHelper
{
    public static void ValidateMappedEndpoints(this Dictionary<EndpointInfo, List<MethodBase>> mappedEndpoints, IList<(EndpointInfo Endpoint, List<string> Methods)> expected)
    {
        var actual = mappedEndpoints.Keys.Select(e => (e, ToStringList(mappedEndpoints[e]))).ToList();

        actual.Should().BeEquivalentTo(expected);
    }

    private static IList<string> ToStringList(IEnumerable<MethodBase> methods) => methods.Select(m => m.Name).ToList();
}
