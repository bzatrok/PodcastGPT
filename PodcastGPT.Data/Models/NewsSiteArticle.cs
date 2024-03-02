namespace PodcastGPT.Data.Models;

public class NewsSiteArticle
{
	public Guid NewsSiteArticleId { get; set; }
	public DateTime PublishDate { get; set; }
	public string Title { get; set; }
	public string Summary { get; set; }
	public string Url { get; set; }
	// public NewsSite NewsSite { get; set; }
	// public Guid NewsSiteId { get; set; }
	
	public virtual Podcast Podcast { get; set; }
	public Guid? PodcastId { get; set; }
}