
using System.Text.Json.Serialization;

namespace PodcastGPT.Core.Models.TextToSpeech;

public class ElevenlabsRequest
{
	public string ModelId { get; set; }
	public List<PronunciationDictionaryLocator> PronunciationDictionaryLocators { get; set; } = new List<PronunciationDictionaryLocator>();
	public string Text { get; set; }
	public VoiceSettings VoiceSettings { get; set; }
}

public class PronunciationDictionaryLocator
{
	public string PronunciationDictionaryId { get; set; }
	public string VersionId { get; set; }
}

public class VoiceSettings
{
	public double SimilarityBoost { get; set; }
	public double Stability { get; set; }
	public double Style { get; set; }
	public bool UseSpeakerBoost { get; set; }
}