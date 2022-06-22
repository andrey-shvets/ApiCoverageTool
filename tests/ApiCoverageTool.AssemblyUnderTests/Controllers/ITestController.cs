using System.Threading.Tasks;
using RestEase;

namespace ApiCoverageTool.AssemblyUnderTests.Controllers
{
    [BasePath("/api/Operation")]
    public interface ITestController
    {
        [Get]
        Task<object> GetNoPathMethod();

        [Get("/")]
        Task<object> GetEmptyPathMethod();

        [Get("get")]
        Task<object> GetMethod();

        [Get("/All/")]
        Task<object> GetAllMethod([Query] string parameter);

        [Post("all/duplicate")]
        Task<object> PostMethod([Body] string parameter);

        [Post("/all/duplicate")]
        Task<object> PostDuplicateMethod([Body] string parameter = null);

        [Put("all")]
        Task<object> PutAllMethod([Query] string parameter);

        [Patch("all/")]
        Task<object> PatchAllMethod();

        [Delete("all")]
        Task<object> DeleteAllMethod([Query] string parameter);

        [Put("withParameters?api-version=1.0")]
        Task CallEndpointWithParameters();

        [Put("withParametersNotTested?api-version=1.0")]
        Task CallEndpointWithParametersNotTested();

        Task<object> NonRestMethod();
    }
}
