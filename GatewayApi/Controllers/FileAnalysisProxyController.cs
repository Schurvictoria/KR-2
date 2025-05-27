using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("files/analysis")]
    public class FileAnalysisProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FileAnalysisProxyController> _logger;

        public FileAnalysisProxyController(IHttpClientFactory httpClientFactory, ILogger<FileAnalysisProxyController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("FileAnalysis");
            _logger = logger;
        }

        private async Task<HttpResponseMessage> ForwardAsync(HttpMethod method, string path)
        {
            var forwardRequest = new HttpRequestMessage(method, path)
            {
                Content = (method == HttpMethod.Get || method == HttpMethod.Head)
                    ? null
                    : new StreamContent(Request.Body)
            };

            foreach (var header in Request.Headers)
            {
                if (!forwardRequest.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
                {
                    forwardRequest.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            _logger.LogInformation("Forwarding {Method} {Path}", method, path);
            return await _httpClient.SendAsync(forwardRequest, HttpCompletionOption.ResponseHeadersRead);
        }

        [HttpPost("analyze/{id}")]
        public async Task<IActionResult> Analyze(string id)
        {
            var path = $"/files/analysis/analyze/{id}";
            var response = await ForwardAsync(HttpMethod.Post, path);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Analyze proxy failed {Status}: {Body}", response.StatusCode, body);
                return StatusCode((int)response.StatusCode, body);
            }

            return Content(body, response.Content.Headers.ContentType?.MediaType);
        }

        [HttpGet("result/{id}")]
        public async Task<IActionResult> GetResult(string id)
        {
            var path = $"/files/analysis/result/{id}";
            var response = await ForwardAsync(HttpMethod.Get, path);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GetResult proxy failed {Status}: {Body}", response.StatusCode, body);
                return StatusCode((int)response.StatusCode, body);
            }

            return Content(body, response.Content.Headers.ContentType?.MediaType);
        }

        [HttpGet("cloud/{*location}")]
        public async Task<IActionResult> GetCloud(string location)
        {
            var path = $"/files/analysis/cloud/{location}";
            var response = await ForwardAsync(HttpMethod.Get, path);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GetCloud proxy failed {Status}", response.StatusCode);
                return StatusCode((int)response.StatusCode);
            }

            var stream = await response.Content.ReadAsStreamAsync();
            return File(stream, response.Content.Headers.ContentType?.MediaType);
        }

        [HttpPost("plagiarism/{idA}/{idB}")]
        public async Task<IActionResult> ComparePlagiarism(string idA, string idB, [FromQuery] int k = 5)
        {
            var path = $"/files/analysis/plagiarism/{idA}/{idB}?k={k}";
            var response = await ForwardAsync(HttpMethod.Post, path);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("ComparePlagiarism proxy failed {Status}: {Body}", response.StatusCode, body);
                return StatusCode((int)response.StatusCode, body);
            }

            return Content(body, response.Content.Headers.ContentType?.MediaType);
        }
    }
}
