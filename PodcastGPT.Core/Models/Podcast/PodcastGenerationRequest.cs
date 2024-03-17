namespace PodcastGPT.Core.Models;

public class PodcastGenerationRequest
{
	// public Guid? PodcastHostPersonaId { get; set; }
	// public Guid? PodcastGuestPersonaId { get; set; }
	// public List<string>? PodcastNewsArticleUrls { get; set; }
	// public string? PodcastTitle { get; set; }
	public string? PodcastTopic { get; set; }
	public string? PodcastNewsArticleUrl { get; set; }
	public bool ShouldGenerate { get; set; }
}