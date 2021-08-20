using System;
using System.Threading.Tasks;
using ApiCoverageTool.AssemblyUnderTests.Controllers;
using RestEase;
using Xunit;

namespace ApiCoverageTool.AssemblyUnderTests
{
    public class MockTests
    {
        private readonly ITestController client = RestClient.For<ITestController>();

        [Fact]
        public async Task MockFactAsync()
        {
            await NotTestMethodAsync();
            var _ = await client.GetMethod();
            NotTestMethodNoClientCall();
        }

        [Theory]
        [InlineData(null)]
        public void MockTheory(string _)
        {
            var someInt = 42;
            var newClient = RestClient.For<ITestController>();
            var someString = someInt.ToString() + NotTestMethodNoClientCall();
            var result = newClient.GetMethod().Result;
            NotTestMethodNoClientCall();
        }

        [Theory]
        [InlineData(null)]
        public async Task NoClientCallTheoryAsync(string _)
        {
            await NotTestMethodAsync();
        }

        [Fact]
        public void NoClientCallFact()
        {
            NotTestMethodNoClientCall();
        }

        [Fact]
        public void MockLambdaExpression()
        {
            NotTestMethodExecutesLambdaExpression(() => client.PatchAllMethod());
        }

        [Fact]
        public async Task MockLambdaExpressionAsync()
        {
            Func<Task> lambda = async () => await client.PatchAllMethod();
            await lambda();
        }

        public string NotTestMethodNoClientCall()
        {
            return string.Empty;
        }

        public async Task NotTestMethodAsync()
        { }
        
        private void NotTestMethodExecutesLambdaExpression(Func<Task<object>> lambda)
        {
            var _ = lambda().Result;
        }
    }
}
