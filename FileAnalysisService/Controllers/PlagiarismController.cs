using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FileAnalysisService.Application.Dtos;
using FileAnalysisService.Application.Helpers;

namespace FileAnalysisService.Controllers
{
    [ApiController]
    [Route("files/analysis/plagiarism")]
    public class PlagiarismController : ControllerBase
    {
        private readonly HttpClient _storerClient;

        public PlagiarismController(IHttpClientFactory httpClientFactory)
        {
            _storerClient = httpClientFactory.CreateClient("FileStorer");
        }

        [HttpPost("{idA}/{idB}")]
        public async Task<IActionResult> Compare(Guid idA, Guid idB, [FromQuery] int k = 5)
        {
            var respA = await _storerClient.GetAsync($"/files/storage/files/{idA}");
            var respB = await _storerClient.GetAsync($"/files/storage/files/{idB}");
            if (!respA.IsSuccessStatusCode || !respB.IsSuccessStatusCode)
                return NotFound("One or both files not found in storage service");

            var bytesA = await respA.Content.ReadAsByteArrayAsync();
            var bytesB = await respB.Content.ReadAsByteArrayAsync();
            var textA = System.Text.Encoding.UTF8.GetString(bytesA);
            var textB = System.Text.Encoding.UTF8.GetString(bytesB);

            var sim = ShinglingHelper.JaccardSimilarity(textA, textB, k);
            var dto = new PlagiarismResultDto
            {
                FileId = $"{idA}/{idB}",
                Similarity = Math.Round(sim, 4),
                Threshold = 0.7
            };
            return Ok(dto);
        }
    }
}
