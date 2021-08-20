using System.Net.Http;

namespace ApiCoverageTool.Models
{
    public record EndpointInfo
    {
        public HttpMethod RestMethod { get; init; }
        public string Path { get; init; }

        public EndpointInfo(HttpMethod restMethod, string path)
        {
            RestMethod = restMethod;
            Path = path;
        }

        public override string ToString() => $"{RestMethod.ToString().ToUpper()} {Path}";
    }
}
