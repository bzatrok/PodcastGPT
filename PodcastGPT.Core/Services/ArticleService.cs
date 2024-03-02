using PodcastGPT.Core.Clients;
using PodcastGPT.Data.Models;
using HtmlAgilityPack;
using PodcastGPT.Core.Helpers;

namespace PodcastGPT.Core.Services;

public class ArticleService
{
	private readonly OpenAiClient _openAiClient;
	
	public ArticleService(OpenAiClient openAiClient)
	{
		_openAiClient = openAiClient;
	}
	
	public async Task<List<NewsSiteArticle>> GetArticlesFromUrls(List<string> articleUrls)
	{
		var articleTasks = articleUrls.Select(articleUrl => GetArticleFromUrl(articleUrl));
		var articles = await Task.WhenAll(articleTasks);
		return articles.ToList();
	}
	
	public async Task<NewsSiteArticle> GetArticleFromUrl(string articleUrl)
	{
		var web = new HtmlWeb();
		var doc = await web.LoadFromWebAsync(articleUrl);

		var titleNode = doc.DocumentNode.SelectSingleNode("//h1");
		var publishDateNode = doc.DocumentNode.SelectSingleNode("//time");
		var summaryNode = doc.DocumentNode.SelectSingleNode("//meta[@name='description']");
		
		var article = new NewsSiteArticle
		{
			Title = titleNode?.InnerText.Replace("\n","").Trim(),
			Url = articleUrl,
			Summary = summaryNode?.GetAttributeValue("content", string.Empty).Trim(),
			PublishDate = DateTime.TryParse(publishDateNode?.GetAttributeValue("datetime", string.Empty), out var publishDate) ? publishDate : DateTime.MinValue,
		};
		
		var requestHash = HashHelper.GenerateHash(article);

		article.NewsSiteArticleId = new Guid(requestHash.Substring(0, 32));
		
		// var systemMessage = "You are an expert summarizer of articles so people running podcasts can talk about them & ask questions about the articles. Please summarize the article for me.";
		// var summarizedContent = await _openAiClient.GenerateSingleAssistantResponse(systemMessage, article.Content);
		
		return article;
	}
}