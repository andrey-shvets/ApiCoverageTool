using System.Threading.Tasks;
using RestEase;

namespace ApiCoverageTool.AssemblyUnderTests.Controllers
{
    [BasePath("/api/operation")]
    public interface ITestControllerNoMethods
    {
        Task<object> MockMethod();
    }
}
