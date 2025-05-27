using System.Text.Json.Serialization;

namespace FileAnalysisService.Application.Dtos
{
	public class PlagiarismResultDto
	{
		[JsonPropertyName("fileId")] public string FileId { get; set; } = default!;
		[JsonPropertyName("similarity")] public double Similarity { get; set; }
		[JsonPropertyName("threshold")] public double Threshold { get; set; }
	}
}