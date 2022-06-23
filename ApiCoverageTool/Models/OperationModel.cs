using System.Text.Json.Serialization;

namespace ApiCoverageTool.Models
{
    public class OperationModel
    {
        [JsonPropertyName("operationId")]
        public string OperationId { get; set; }
    }
}
