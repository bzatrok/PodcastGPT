namespace PodcastGPT.Data.Models;

public class NewsSiteAuthor
{
	public Guid NewsSiteAuthorId { get; set; }
	public string Name { get; set; }
	public virtual ICollection<NewsSiteArticle> NewsSiteArticles { get; set; }	= new List<NewsSiteArticle>();
	public virtual NewsSite NewsSite { get; set; }
	public Guid NewsSiteId { get; set; }
}
