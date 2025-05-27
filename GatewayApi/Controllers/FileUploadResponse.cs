using System.Text.Json.Serialization;

namespace ApiGateway.Models
{
    public class FileUploadResponse
    {
        [JsonPropertyName("id")]
        public string FileId { get; set; } = default!;
    }
}
