using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WordCloudService.Models;

namespace WordCloudService.Services
{
    public class WordCloudeService : IWordCloudeService
    {
        private readonly HttpClient _httpClient;
        public WordCloudeService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("QuickChart");
        }

        public async Task<byte[]> GenerateAsync(CloudRequestDto request)
        {
            var words = request.Words.Select(w => new { text = w.Text, value = w.Weight }).ToArray();
            
            var config = new
            {
                type = "wordCloud",
                data = new
                {
                    words = words
                },
                options = new
                {
                    title = new
                    {
                        display = true,
                        text = request.Title ?? "Word Cloud",
                        font = new { size = request.TitleFontSize, family = request.FontFamily }
                    },
                    rotation = new { from = request.RotationFrom, to = request.RotationTo, orientations = request.OrientationCount },
                    colors = request.Colors,
                    minRotation = request.RotationFrom,
                    maxRotation = request.RotationTo,
                    fontFamily = request.FontFamily,
                    fontSizes = new[] { 14, 60 }
                }
            };

            var jsonConfig = JsonSerializer.Serialize(config);
            var url = $"/chart?width={request.Width}&height={request.Height}&format={request.Format}&c={Uri.EscapeDataString(jsonConfig)}";
            return await _httpClient.GetByteArrayAsync(url);
        }
    }
}
