using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WordCloudService.Models;
using WordCloudService.Services;

namespace WordCloudService.Controllers
{
    [ApiController]
    [Route("wordcloud")]
    public class CloudController : ControllerBase
    {
        private readonly IWordCloudeService _service;
        private readonly ILogger<CloudController> _logger;

        public CloudController(IWordCloudeService service, ILogger<CloudController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CloudRequestDto request)
        {
            _logger.LogInformation("Получен запрос на облако слов: {@Request}", request);
            var image = await _service.GenerateAsync(request);
            return File(image, $"image/{request.Format}");
        }
    }
}
