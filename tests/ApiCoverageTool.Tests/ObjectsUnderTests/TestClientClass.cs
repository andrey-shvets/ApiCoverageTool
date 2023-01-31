using System.Threading.Tasks;
using RestEase;

namespace ApiCoverageTool.Tests.ObjectsUnderTests;

public class TestClientClass
{
    [Get]
    public Task<object> GetNoPathMethod() => new Task<object>(() => null);

    public Task<object> GetNoPathMethod(object obj) => Task.FromResult(obj);
}
