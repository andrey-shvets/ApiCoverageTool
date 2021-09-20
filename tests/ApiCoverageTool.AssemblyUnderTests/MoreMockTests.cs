using System.Threading.Tasks;
using ApiCoverageTool.AssemblyUnderTests.Controllers;
using RestEase;
using Xunit;

namespace ApiCoverageTool.AssemblyUnderTests
{
    [Trait("Category", "Mock tests")]
    public class MoreMockTests
    {
        private readonly ITestController _client = RestClient.For<ITestController>();

        [Fact]
        public async Task MockTestNestedCallAsync()
        {
            await Task.Delay(1);
            var newClient = RestClient.For<ITestController>();
            var _ = string.Empty + $"{4}{2}"
                + newClient.GetNoPathMethod().ToString()
                + newClient.NonRestMethod().Result.ToString();
            await NotTestMethodAsync(newClient);
        }

        [Fact]
        public void MockTestNestedCall()
        {
            NotTestMethod();
            new MockTests().NotTestMethodNoClientCall();
            var someInt = 42;
            var str = someInt.ToString(nameof(someInt));
            _ = str.ToLower();
            var newClient = RestClient.For<ITestController>();
            _ = newClient.PatchAllMethod().Result;
        }

        [Fact]
        public void MockTestDifferentClassStaticCall()
        {
            MockClass.StaticEndpointCall();
        }

        [Fact]
        public void MockTestDifferentClassCall()
        {
            new MockClass().EndpointCall();
        }

        [Fact]
        public void MockTestWithCycleInCallTree()
        {
            MethodWithRecursion(42);
            MethodWithTheCycle();
        }

        private string MethodWithRecursion(int i)
        {
            if (i > 0)
                return MethodWithRecursion(i - 1);
            else
                throw new System.NotImplementedException($"{NotTestMethod()}");
        }

        public void MethodWithTheCycle()
        {
            MethodWithTheCycle2();
        }

        public void MethodWithTheCycle2()
        {
            MethodWithTheCycle3();
        }

        public void MethodWithTheCycle3()
        {
            MethodWithTheCycle();
        }

        private object NotTestMethod()
        {
            return _client.GetMethod().Result;
        }

        private static async Task<object> NotTestMethodAsync(ITestController newClient)
        {
            return await newClient.GetMethod();
        }
    }
}
