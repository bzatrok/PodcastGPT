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
	public virtual ICollection<PodcastSegment> PodcastSegments { get; set; } = new List<PodcastSegment>();
	public virtual ICollection<PodcastPersona> PodcastPersonas { get; set; } = new List<PodcastPersona>();
	public virtual ICollection<NewsSiteArticle> NewsSiteArticles { get; set; } = new List<NewsSiteArticle>();
}