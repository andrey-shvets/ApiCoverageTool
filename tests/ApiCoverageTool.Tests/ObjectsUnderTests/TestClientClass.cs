using System.Threading.Tasks;
using RestEase;

namespace ApiCoverageTool.Tests.ObjectsUnderTests
{
    public class TestClientClass
    {
        [Get]
        public Task<object> GetNoPathMethod()
        {
            return null;
        }

        public Task<object> GetNoPathMethod(object obj)
        {
            return Task.FromResult(obj);
        }
    }
}
