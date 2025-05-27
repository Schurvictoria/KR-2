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
            var config = new
            {
                type = "wordCloud",
                data = new
                {
                    labels = request.Words.Select(w => w.Text),
                    datasets = new[] { new { label = "Word Cloud", data = request.Words.Select(w => w.Weight) } }
                },
                options = new
                {
                    title = new
                    {
                        display = true,
                        text = request.Title ?? "Word Cloud",
                        font = new { size = request.TitleFontSize, family = request.FontFamily }
                    },
                    backgroundColor = request.BackgroundColor,
                    rotation = new { from = request.RotationFrom, to = request.RotationTo, orientations = request.OrientationCount },
                    colors = request.Colors
                }
            };

            var jsonConfig = JsonSerializer.Serialize(config);
            var encoded = Uri.EscapeDataString(jsonConfig);
            var url = $"/chart?width={request.Width}&height={request.Height}&format={request.Format}&backgroundColor={Uri.EscapeDataString(request.BackgroundColor)}&chart={encoded}";
            return await _httpClient.GetByteArrayAsync(url);
        }
    }
}
