using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApiGateway.Models;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("files/storage")]
    public class FileStorageProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public FileStorageProxyController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FileStorage");
        }

        [HttpPost("upload")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Upload()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/files/storage/upload");
            requestMessage.Content = new StreamContent(Request.Body);
            requestMessage.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(Request.ContentType ?? "application/octet-stream");

            var response = await _httpClient.SendAsync(requestMessage);
            var content = await response.Content.ReadAsStringAsync();
            return Content(content, response.Content.Headers.ContentType?.MediaType);
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] FileUploadRequest request)
        {
            var file = request.File;
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            using var content = new MultipartFormDataContent();
            await using var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

            content.Add(fileContent, "file", file.FileName);

            var response = await _httpClient.PostAsync("/files/storage/upload", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            return Content(responseContent, response.Content.Headers.ContentType?.MediaType ?? "text/plain");
        }

        [HttpGet("files/{id}")]
        public async Task<IActionResult> GetFile(string id)
        {
            var response = await _httpClient.GetAsync($"/files/storage/files/{id}");
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var stream = await response.Content.ReadAsStreamAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            return File(stream, contentType);
        }
    }
}
