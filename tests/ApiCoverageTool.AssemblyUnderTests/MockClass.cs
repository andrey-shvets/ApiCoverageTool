using ApiCoverageTool.AssemblyUnderTests.Controllers;
using RestEase;

namespace ApiCoverageTool.AssemblyUnderTests;

public class MockClass
{
    public static void StaticEndpointCall()
    {
        var newClient = RestClient.For<ITestController>();
        _ = newClient.GetAllMethod("").Result;
    }

    public void EndpointCall()
    {
        var newClient = RestClient.For<ITestController>();
        _ = newClient.GetAllMethod("").Result;
    }

    public static void StaticMethod()
    {
        _ = typeof(object).FullName;
    }

    public void Method()
    {
        _ = this.GetType().Assembly.GetName().FullName;
    }
}
