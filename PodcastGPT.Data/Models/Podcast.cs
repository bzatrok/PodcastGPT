namespace PodcastGPT.Data.Models;

public class Podcast
{
	public Guid PodcastId { get; set; }

	public string Slug { get; set; }
	public string Status { get; set; }
	public string Title { get; set; }
	public string Topic { get; set; }
	public DateTime Date { get; set; }
	public string? FullAudioFileUrl { get; set; }
	public List<PodcastSegment> PodcastSegments { get; set; }
	public List<PodcastPersona> PodcastPersonas { get; set; }
	public List<NewsSiteArticle> NewsSiteArticles { get; set; }
}