using Microsoft.AspNetCore.Http;

namespace ApiGateway.Models
{
    public class FileUploadRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
