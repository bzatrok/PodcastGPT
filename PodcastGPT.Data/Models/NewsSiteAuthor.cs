namespace PodcastGPT.Data.Models;

public class NewsSiteAuthor
{
	public Guid NewsSiteAuthorId { get; set; }
	public string Name { get; set; }
	public List<NewsSiteArticle> NewsSiteArticles { get; set; }
	public NewsSite NewsSite { get; set; }
	public Guid NewsSiteId { get; set; }
}
