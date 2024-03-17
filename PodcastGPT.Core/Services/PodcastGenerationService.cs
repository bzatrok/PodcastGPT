using PodcastGPT.Core.Clients;
using PodcastGPT.Core.Models;
using PodcastGPT.Core.Repositories;
using PodcastGPT.Data.Models;
using Microsoft.Extensions.Logging;
using PodcastGPT.Core.Constants;
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
	private readonly IGenericRepository<PodcastSegment> _podcastSegmentRepository;
	// private readonly RssService _rssService;
	private readonly OpenAIService _openAiService;
	private readonly OpenAiClient _openAiClient;
	private readonly ArticleService _articleService;
	private readonly AudioService _audioService;
	
	private bool _isGeneratingPodcast = false;

	public PodcastGenerationService(
		ILogger<PodcastGenerationService> logger,
		// IGenericRepository<NewsSite> newsSiteRepository,
		IGenericRepository<NewsSiteArticle> newsSiteArticleRepository,
		IGenericRepository<Podcast> podcastRepository,
		IGenericRepository<PodcastPersona> podcastPersonaRepository,
		IGenericRepository<PodcastSegment> podcastSegmentRepository,
		// RssService rssService,
		OpenAIService openAiService,
		OpenAiClient openAiClient,
		ArticleService articleService,
		AudioService audioService
		)
	{
		_logger = logger;
		// _newsSiteRepository = newsSiteRepository;
		_newsSiteArticleRepository = newsSiteArticleRepository;
		_podcastRepository = podcastRepository;
		_podcastPersonaRepository = podcastPersonaRepository;
		_podcastSegmentRepository = podcastSegmentRepository;
		// _rssService = rssService;
		_openAiService = openAiService;
		_openAiClient = openAiClient;
		_articleService = articleService;
		_audioService = audioService;
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

			string podcastTitle = podcastGenerationRequest.PodcastTopic;

			if (podcastGenerationRequest.ShouldGenerate)
			{
				if (!string.IsNullOrWhiteSpace(podcastGenerationRequest.PodcastNewsArticleUrl))
					newsArticlesForPodcast = await _articleService.GetArticlesFromUrls(new List<string> { podcastGenerationRequest.PodcastNewsArticleUrl });

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
				podcastTitle = await _openAiClient.GenerateSingleAssistantResponse(summaryGenSystemPrompt, kickoffMessage);
			}

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

			if (podcastGenerationRequest.ShouldGenerate)
			{
				Task.Run(async () =>
				{
					await GeneratePodcastContentFromNews(
						newPodcastId,
						// podcastTitle,
						// podcastGenerationRequest.PodcastTopic,
						newsArticlesForPodcast
					);
				});
			}
			else
			{
				newPodcast.Status = "ready";
				await _podcastRepository.SaveChangesAsync();
			}

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
	
	public async Task<Podcast> GeneratePodcastContentFromNews(
		Guid podcastId,
		// string? podcastTitle,
		// string? podcastTopic,
		List<NewsSiteArticle> articleList
		)
	{
		var podcast = await _podcastRepository.GetByIdAsync(podcastId);
		var showName = "NewsFlash";

		var podcastArticles = string.Join(';', articleList.Select(article => $"Title: {article.Title}. Summary: {article.Summary}"));
		var newsSection = articleList.Count > 0 ? $" We have a lot of news to cover today. Some of our highlights: {podcastArticles}" : "";
		
		var interviewerSystemMessage = $"{OpenAISystemMessages.InterviewerSystemMessage}{newsSection}";
		var guestSystemMessage = OpenAISystemMessages.GuestSystemMessage;

		List<PodcastPersona> allPersonas = await _podcastPersonaRepository.GetAllAsync();
		 	
		PodcastPersona? hostPersona = allPersonas.FirstOrDefault(persona => persona.PodcastPersonaId == podcast.PodcastHostPersonaId);
		PodcastPersona? guestPersona = allPersonas.FirstOrDefault(persona => persona.PodcastPersonaId == podcast.PodcastGuestPersonaId);

		// var hostPersona = podcast.PodcastHostPersona;
		// var guestPersona = podcast.PodcastGuestPersona;
		
		var personas = new List<PodcastPersona> { hostPersona, guestPersona };
		
		// SETTING UP PODCAST LOGIC
		
		// var existingPodcast = await _podcastRepository.GetByIdAsync(podcast.PodcastId);
		
		// Re-create if exists
		// if (existingPodcast != null)
		// 	_podcastRepository.Delete(existingPodcast.PodcastId);
		
		// podcast.PodcastPersonas.Add(hostPersona);
		// podcast.PodcastPersonas.Add(guestPersona);

		// podcast.PodcastHostPersonaId = hostPersona.PodcastPersonaId;
		// podcast.PodcastGuestPersonaId = guestPersona.PodcastPersonaId;

		var firstSegment = new PodcastSegment
		{
			Order = 1,
			OpenAiRole = "user",
			TextContent =
				$"Good morning folks! Welcome to the latest episode of our podcast, {showName}. I'm your host, {hostPersona.Name}. Today's topic is {podcast.Topic}. We have a special guest with us, {guestPersona.Name}. Welcome to the show!",
			PodcastId = podcast.PodcastId,
			PodcastPersonaId = hostPersona.PodcastPersonaId,
			PodcastSegmentId = Guid.NewGuid()
		};
		
		await _podcastSegmentRepository.AddAsync(firstSegment);

		var previousSegment = firstSegment;
		
		var segments = new List<PodcastSegment>
		{
			firstSegment
		};

		while (!previousSegment.TextContent.Contains("__OVER__"))
		{
			var nextPersona = previousSegment.PodcastPersonaId == hostPersona.PodcastPersonaId
				? guestPersona
				: hostPersona;
			
			var nextRole = previousSegment.PodcastPersonaId == hostPersona.PodcastPersonaId
				? "assistant"
				: "user";
			
			var systemMessage = previousSegment.PodcastPersonaId == hostPersona.PodcastPersonaId
				? guestSystemMessage
				: interviewerSystemMessage;
			
			var nextSegment = await _openAiService.GenerateAssistantResponseForPodcastSegments(systemMessage, segments);

			if (nextSegment == null)
				throw new Exception("OpenAI response is null");
			
			// await _podcastRepository.SaveChangesAsync();

			var newSegment = new PodcastSegment
			{
				Order = previousSegment.Order + 1,
				OpenAiRole = nextRole,
				TextContent = nextSegment,
				PodcastId = podcast.PodcastId,
				PodcastPersonaId = nextPersona.PodcastPersonaId,
				PodcastSegmentId = Guid.NewGuid()
			};
			
			await _podcastSegmentRepository.AddAsync(newSegment);

			segments.Add(newSegment);
			
			previousSegment = newSegment;
		}
		
		podcast.Status = "textready";

		await _podcastRepository.SaveChangesAsync();

		foreach (var segment in segments)
		{
			var persona = personas.FirstOrDefault(persona => persona.PodcastPersonaId == segment.PodcastPersonaId);
			var generatedAudioForSegment = await _openAiService.ConvertSegmentToSpeech(segment, persona.VoiceId);
			segment.AudioFileUrl = generatedAudioForSegment.AudioUrl;
			await _podcastSegmentRepository.SaveChangesAsync();
		}
		
		podcast.Status = "audiosegmentsready";
		
		await _podcastRepository.SaveChangesAsync();

		var mergedFilePath = await _audioService.MergeAudioFilesForPodcast(podcast.PodcastId);
		
		podcast.FullAudioFileUrl = mergedFilePath;

		podcast.Status = "ready";
		await _podcastRepository.SaveChangesAsync();
		
		return podcast;
	}
}