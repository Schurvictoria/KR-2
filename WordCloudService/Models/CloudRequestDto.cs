using System.Collections.Generic;

namespace WordCloudService.Models
{
    public class CloudRequestDto
    {
        public string Format { get; set; } = "png";
        public int Width { get; set; } = 800;
        public int Height { get; set; } = 600;

        public string? Title { get; set; }
        public int TitleFontSize { get; set; } = 24;
        public string FontFamily { get; set; } = "Arial";

        public string BackgroundColor { get; set; } = "transparent";
        public string[] Colors { get; set; } = new[] { "#336699", "#669933", "#993366", "#cc3333", "#33cccc" };

        public int RotationFrom { get; set; } = 0;
        public int RotationTo { get; set; } = 90;
        public int OrientationCount { get; set; } = 4;

        public List<WordDto> Words { get; set; } = new();
    }
}
