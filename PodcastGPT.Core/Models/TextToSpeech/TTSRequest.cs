using System.Text.Json.Serialization;

namespace PodcastGPT.Core.Models.TextToSpeech;

public class TTSRequest
{
	public string Text { get; set; }
}