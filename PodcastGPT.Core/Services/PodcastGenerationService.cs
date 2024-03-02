using PodcastGPT.Core.Clients;
using PodcastGPT.Core.Models;
using PodcastGPT.Core.Repositories;
using PodcastGPT.Data.Models;
using Microsoft.Extensions.Logging;
using PodcastGPT.Core.Extensions;
using PodcastGPT.Core.Helpers;

namespace PodcastGPT.Core.Services;

public class PodcastGenerationService
{
	private readonly ILogger<PodcastGenerationService> _logger;
	// private readonly IGenericRepository<NewsSite> _newsSiteRepository;
	private readonly IGenericRepository<NewsSiteArticle> _newsSiteArticleRepository;
	private readonly IGenericRepository<Podcast> _podcastRepository;
	private readonly IGenericRepository<PodcastPersona> _podcastPersonaRepository;
	// private readonly RssService _rssService;
	private readonly OpenAIService _openAiService;
	private readonly OpenAiClient _openAiClient;
	private readonly ArticleService _articleService;
	
	private bool _isGeneratingPodcast = false;

	public PodcastGenerationService(
		ILogger<PodcastGenerationService> logger,
		// IGenericRepository<NewsSite> newsSiteRepository,
		IGenericRepository<NewsSiteArticle> newsSiteArticleRepository,
		IGenericRepository<Podcast> podcastRepository,
		IGenericRepository<PodcastPersona> podcastPersonaRepository,
		// RssService rssService,
		OpenAIService openAiService,
		OpenAiClient openAiClient,
		ArticleService articleService
		)
	{
		_logger = logger;
		// _newsSiteRepository = newsSiteRepository;
		_newsSiteArticleRepository = newsSiteArticleRepository;
		_podcastRepository = podcastRepository;
		_podcastPersonaRepository = podcastPersonaRepository;
		// _rssService = rssService;
		_openAiService = openAiService;
		_openAiClient = openAiClient;
		_articleService = articleService;
	}

	public async Task<Podcast> GeneratePodcast(PodcastGenerationRequest podcastGenerationRequest)
	{
		Console.Write("PodcastGenerationTask is running " + DateTime.Now.ToString());

		if (_isGeneratingPodcast)
		{
			Console.Write("PodcastGenerationTask is already running " + DateTime.Now.ToString());
			return null;
		}

		try
		{
			_isGeneratingPodcast = true;
			
			List<NewsSiteArticle> newsArticlesForPodcast = new List<NewsSiteArticle>();
			
			// if (podcastGenerationRequest.PodcastNewsArticleUrl == null || podcastGenerationRequest.PodcastNewsArticleUrls.Count == 0)
			// {
			// 	// Get sites to generate podcast from
			// 	var newsSiteList = await _newsSiteRepository.GetAllAsync();
			//
			// 	// Refresh articles from feeds
			// 	foreach (var newsSite in newsSiteList)
			// 	{
			// 		var newArticles = await _rssService.GetNewArticlesForRssFeedUrl(newsSite.RssFeedUrl);
			//
			// 		foreach (var newArticle in newArticles)
			// 		{
			// 			newArticle.NewsSiteId = newsSite.NewsSiteId;
			// 			await _newsSiteArticleRepository.InsertAsync(newArticle);
			// 		}
			// 	}
			// 	
			// 	// Get articles from sites
			// 	newsArticlesForPodcast = await _newsSiteArticleRepository.GetAllAsync();
			// }
			// else
			// {
			
			// --
			
			if (!string.IsNullOrWhiteSpace(podcastGenerationRequest.PodcastNewsArticleUrl))
				newsArticlesForPodcast = await _articleService.GetArticlesFromUrls(new List<string> {podcastGenerationRequest.PodcastNewsArticleUrl});
			
			foreach (var newArticle in newsArticlesForPodcast)
			{
				var existingArticle = await _newsSiteArticleRepository.GetByIdAsync(newArticle.NewsSiteArticleId);
				if (existingArticle == null)
					await _newsSiteArticleRepository.AddAsync(newArticle);
			}
			
			newsArticlesForPodcast = newsArticlesForPodcast
				.Where(article => article.PodcastId == null)
				.Take(3)
				.ToList();
			
			var kickoffMessage = "Today's news stories are: " + string.Join("; ", newsArticlesForPodcast.Select(article => article.Title));
			var summaryGenSystemPrompt =
				"You are an expert summarizer of news! Please come up with a catchy, slightly clickbait-y title for a podcast based on these news stories! Keep it short, one sentence! Make sure it not too generic! Feature one of the stories in the list provided! " +
				kickoffMessage;
			var podcastTitle = await _openAiClient.GenerateSingleAssistantResponse(summaryGenSystemPrompt, kickoffMessage);
			
			var newPodcast = new Podcast()
			{
				Title = podcastTitle.Trim(),
				Topic = podcastGenerationRequest.PodcastTopic.Trim(),
				Slug = podcastTitle.Slugify(),
			};
		
			var podcastHash = HashHelper.GenerateHash(newPodcast);
			var newPodcastId = new Guid(podcastHash.Substring(0, 32));

			newPodcast.PodcastId = newPodcastId;
			newPodcast.PodcastSegments = new List<PodcastSegment>();
			newPodcast.NewsSiteArticles = new List<NewsSiteArticle>();
			newPodcast.Date = DateTime.Now;
			newPodcast.Status = "generating";
			
			List<PodcastPersona> personas = await _podcastPersonaRepository.GetAllAsync();
			
			PodcastPersona? hostPersona = personas.FirstOrDefault(persona => persona.Type == "interviewer");
			PodcastPersona? guestPersona = personas.FirstOrDefault(persona => persona.Type == "guest");
			
			newPodcast.PodcastGuestPersonaId = guestPersona.PodcastPersonaId;
			newPodcast.PodcastHostPersonaId = hostPersona.PodcastPersonaId;
			
			await _podcastRepository.AddAsync(newPodcast);

			// UPDATE ARTICLES TO MARK AS USED
			foreach (var article in newsArticlesForPodcast)
			{
				article.PodcastId = newPodcast.PodcastId;
			}
			
			await _newsSiteArticleRepository.SaveChangesAsync();

			Task.Run(async () =>
			{
				await _openAiService.GeneratePodcastContentFromNews(
					newPodcastId,
					// podcastTitle,
					// podcastGenerationRequest.PodcastTopic,
					newsArticlesForPodcast
				);
			});

			// OpenAI service -> generate conversation from news
			// var newPodcast = await _openAiService.GeneratePodcastFromNews(
			// 	podcastTitle,
			// 	podcastGenerationRequest.PodcastTopic,
			// 	newsArticlesForPodcast
			// );

			return newPodcast;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred.");
		}
		finally
		{
			_isGeneratingPodcast = false;
		}

		return null;
	}
}