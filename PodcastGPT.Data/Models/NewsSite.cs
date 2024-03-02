namespace PodcastGPT.Data.Models;

public class NewsSite
{
	public Guid NewsSiteId { get; set; }
	public string Name { get; set; }
	public string Url { get; set; }
	public string RssFeedUrl { get; set; }
	public virtual ICollection<NewsSiteAuthor> NewsArticleAuthors { get; set; } = new List<NewsSiteAuthor>();
	public virtual ICollection<NewsSiteArticle> NewsSiteArticles { get; set; } = new List<NewsSiteArticle>();
}
