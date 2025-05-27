using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FileAnalysisService.Application.Services;

namespace FileAnalysisService.Controllers
{
    [ApiController]
    [Route("files/analysis")]
    public class AnalysisController : ControllerBase
    {
        private readonly IFileAnalyseService _service;
        private readonly ILogger<AnalysisController> _logger;

        public AnalysisController(IFileAnalyseService service, ILogger<AnalysisController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("analyze/{id}")]
        [HttpPost("analyze/{id}")]
        public async Task<IActionResult> Analyze(Guid id)
        {
            _logger.LogInformation("Analyze called for id={Id}", id);
            try
            {
                var result = await _service.AnalyzeAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("Analysis result is null for id={Id}", id);
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing file {Id}", id);
                return StatusCode(500, "An internal error occurred while processing your request.");
            }
        }

        [HttpGet("result/{id}")]
        public async Task<IActionResult> GetResult(Guid id)
        {
            _logger.LogInformation("GetResult called for id={Id}", id);
            try
            {
                var result = await _service.GetResultAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("No analysis result found for id={Id}", id);
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting analysis result for id={Id}", id);
                return StatusCode(500, "An internal error occurred while processing your request.");
            }
        }

        [HttpGet("cloud/{*location}")]
        public IActionResult GetCloud(string location)
        {
            _logger.LogInformation("GetCloud called for location={Location}", location);
            var path = Path.Combine("storage", location);
            if (!System.IO.File.Exists(path))
            {
                _logger.LogWarning("File not found at path={Path}", path);
                return NotFound();
            }

            var bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "image/png");
        }
    }
}
