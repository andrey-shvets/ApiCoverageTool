using System.Threading.Tasks;
using RestEase;

namespace ApiCoverageTool.AssemblyUnderTests.Controllers
{
    public interface ITestControllerNoBaseRout
    {
        [Get("api/operation/get")]
        Task<object> GetMethod();

        [Post]
        Task<object> PostMethod();

        [Post("/")]
        Task<object> PostEmptyPathMethod();
    }
}
