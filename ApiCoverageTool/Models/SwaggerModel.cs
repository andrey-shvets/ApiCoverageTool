using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiCoverageTool.Models
{
    public class SwaggerModel
    {
        [JsonPropertyName("paths")]
        public Dictionary<string, Dictionary<string, OperationModel>> Paths { get; set; } = new Dictionary<string, Dictionary<string, OperationModel>>();
    }
}
