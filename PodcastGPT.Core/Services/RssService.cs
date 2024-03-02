using System.ServiceModel.Syndication;
using System.Xml;
using PodcastGPT.Data.Models;

namespace PodcastGPT.Core.Services;

public class RssService
{
	public async Task<List<NewsSiteArticle>> GetNewArticlesForRssFeedUrl(string rssFeedUrl)
	{
		var articleList = new List<NewsSiteArticle>();

		using (var reader = XmlReader.Create(rssFeedUrl))
		{
			SyndicationFeed feed = SyndicationFeed.Load(reader);
			foreach (var item in feed.Items)
			{
				var article = new NewsSiteArticle
				{
					NewsSiteArticleId = Guid.NewGuid(),
					Title = item.Title.Text,
					Url = item.Links.FirstOrDefault(link => link.RelationshipType == "alternate")?.Uri.ToString(),
					Summary = item.Summary.Text,
					PublishDate = item.PublishDate.DateTime,
					// NewsSiteId = Guid.Empty
				};
				articleList.Add(article);
			}
		}

		return articleList;
	}
}