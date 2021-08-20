using ApiCoverageTool.AssemblyUnderTests.Controllers;
using RestEase;

namespace ApiCoverageTool.AssemblyUnderTests
{
    public class MockClass
    {
        public static void StaticEndpointCall()
        {
            var newClient = RestClient.For<ITestController>();
            var _ = newClient.GetAllMethod("").Result;
        }

        public void EndpointCall()
        {
            var newClient = RestClient.For<ITestController>();
            var _ = newClient.GetAllMethod("").Result;
        }

        public static void StaticMethod()
        {
            var _ = typeof(object).FullName;
        }

        public void Method()
        {
            var _ = this.GetType().Assembly.GetName().FullName;
        }
    }
}
