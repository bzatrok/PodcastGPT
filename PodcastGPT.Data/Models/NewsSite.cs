namespace PodcastGPT.Data.Models;

public class NewsSite
{
	public Guid NewsSiteId { get; set; }
	public string Name { get; set; }
	public string Url { get; set; }
	public string RssFeedUrl { get; set; }
	public List<NewsSiteAuthor> NewsArticleAuthors { get; set; }
	public List<NewsSiteArticle> NewsSiteArticles { get; set; }
}
