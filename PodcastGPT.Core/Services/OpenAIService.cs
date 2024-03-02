using PodcastGPT.Core.Clients;
using PodcastGPT.Core.Extensions;
using PodcastGPT.Core.Models.TextToSpeech;
using PodcastGPT.Core.Repositories;
using PodcastGPT.Data.DTOs;
using PodcastGPT.Data.Models;
using Microsoft.Extensions.Logging;
using PodcastGPT.Core.Constants;
using PodcastGPT.Core.Helpers;

namespace PodcastGPT.Core.Services;

public class OpenAIService
{
	#region Variables

	private readonly ILogger<OpenAIService> _logger;
	private readonly IGenericRepository<Podcast> _podcastRepository;
	private readonly IGenericRepository<OpenAiConversation> _openAiConversationRepository;
	private readonly IGenericRepository<OpenAiMessage> _openAiMessageRepository;
	private readonly IGenericRepository<PodcastPersona> _podcastPersonaRepository;
	private readonly IGenericRepository<NewsSiteArticle> _articleRepository;
	private readonly AudioService _audioService;
	private readonly OpenAiClient _openAiClient;

	#endregion
	#region Initialization

	public OpenAIService(
		ILogger<OpenAIService> logger,
		IGenericRepository<Podcast> podcastRepository,
		IGenericRepository<OpenAiConversation> openAiConversationRepository,
		IGenericRepository<OpenAiMessage> openAiMessageRepository,
		IGenericRepository<PodcastPersona> podcastPersonaRepository,
		IGenericRepository<NewsSiteArticle> articleRepository,
		AudioService audioService,
		OpenAiClient openAiClient
	)
	{
		_logger = logger;
		_podcastRepository = podcastRepository;
		_openAiConversationRepository = openAiConversationRepository;
		_openAiMessageRepository = openAiMessageRepository;
		_podcastPersonaRepository = podcastPersonaRepository;
		_articleRepository = articleRepository;
		_audioService = audioService;
		_openAiClient = openAiClient;
	}
	
	#endregion
	#region Requests
	
	public async Task<Podcast> GeneratePodcastContentFromNews(
		Podcast podcast,
		string? podcastTitle,
		string? podcastTopic,
		List<NewsSiteArticle> articleList
		)
	{
		var showName = "NewsFlash";

		var podcastArticles = string.Join(';', articleList.Select(article => $"Title: {article.Title}. Summary: {article.Summary}"));
		var newsSection = articleList.Count > 0 ? $" We have a lot of news to cover today. Some of our highlights: {podcastArticles}" : "";
		
		var interviewerSystemMessage = $"{OpenAISystemMessages.InterviewerSystemMessage}{newsSection}";
		var guestSystemMessage = OpenAISystemMessages.GuestSystemMessage;

		List<PodcastPersona> personas = await _podcastPersonaRepository.GetAllAsync();
			
		PodcastPersona? hostPersona = personas.FirstOrDefault(persona => persona.Type == "interviewer");
		PodcastPersona? guestPersona = personas.FirstOrDefault(persona => persona.Type == "guest");
	
			
		// SETTING UP PODCAST LOGIC
		
		// var existingPodcast = await _podcastRepository.GetByIdAsync(podcast.PodcastId);
		
		// Re-create if exists
		// if (existingPodcast != null)
		// 	_podcastRepository.Delete(existingPodcast.PodcastId);
		
		await _podcastRepository.InsertOrUpdateAsync(podcast.PodcastId, podcast);

		podcast.PodcastSegments.Add(new PodcastSegment
		{
			Order = 1,
			OpenAiRole = "user",
			TextContent = $"Good morning folks! Welcome to the latest episode of our podcast, {showName}. I'm your host, {hostPersona.Name}. Today's topic is {podcastTopic}. We have a special guest with us, {guestPersona.Name}. Welcome to the show!",
			PodcastId = podcast.PodcastId,
			PodcastPersonaId = hostPersona.PodcastPersonaId
		});

		var lastSegment = podcast.PodcastSegments.Last();

		while (!lastSegment.TextContent.Contains("__OVER__"))
		{
			lastSegment = podcast.PodcastSegments.Last();
			
			var nextPersona = lastSegment.PodcastPersonaId == hostPersona.PodcastPersonaId
				? guestPersona
				: hostPersona;
			
			var nextRole = lastSegment.PodcastPersonaId == hostPersona.PodcastPersonaId
				? "assistant"
				: "user";
			
			var systemMessage = lastSegment.PodcastPersonaId == hostPersona.PodcastPersonaId
				? guestSystemMessage
				: interviewerSystemMessage;
			
			var nextSegment = await GenerateAssistantResponseForPodcastSegments(systemMessage, podcast.PodcastSegments);

			if (nextSegment == null)
				throw new Exception("OpenAI response is null");
			
			podcast.PodcastSegments.Add(new PodcastSegment
			{
				Order = lastSegment.Order + 1,
				OpenAiRole = nextRole,
				TextContent = nextSegment,
				PodcastId = podcast.PodcastId,
				PodcastPersonaId = nextPersona.PodcastPersonaId
			});
		}

		podcast.Status = "textready";

		await _podcastRepository.InsertOrUpdateAsync(podcast.PodcastId, podcast);

		foreach (var segment in podcast.PodcastSegments)
		{
			var persona = personas.FirstOrDefault(persona => persona.PodcastPersonaId == segment.PodcastPersonaId);
			
			var generatedAudioForSegment = await ConvertSegmentToSpeech(segment, persona.VoiceId);
			segment.AudioFileUrl = generatedAudioForSegment.AudioUrl;
		}
		
		podcast.Status = "audiosegmentsready";
		await _podcastRepository.InsertOrUpdateAsync(podcast.PodcastId, podcast);

		 var mergedFilePath = await _audioService.MergeAudioFilesForPodcast(podcast);
		
		podcast.FullAudioFileUrl = mergedFilePath;

		podcast.Status = "ready";
		await _podcastRepository.InsertOrUpdateAsync(podcast.PodcastId, podcast);
		
		// UPDATE ARTICLES TO MARK AS USED
		foreach (var article in articleList)
		{
			article.PodcastId = podcast.PodcastId;
			await _articleRepository.InsertOrUpdateAsync(article.NewsSiteArticleId, article);
		}
		
		return podcast;
	}
    
    public async Task<string> GenerateAssistantResponseForPodcastSegments(string systemPrompt, List<PodcastSegment> messages)
    {
	    try
	    {
		    var requestMessages = new List<OpenAiMessageDto>()
		    {
			    new OpenAiMessageDto()
			    {
				    role = "system",
				    content = systemPrompt
			    }
		    };

		    requestMessages.AddRange(messages
			    .Where(segment => segment.OpenAiRole != "system")
			    .Select(segment => new OpenAiMessageDto
			    {
				    content = segment.TextContent.Replace("__OVER__", ""),
				    role = segment.OpenAiRole
			    })
			    .ToList());
		    
		    var requestContent = new OpenAiRequestDto
		    {
			    model = Environment.GetEnvironmentVariable("OPEN_AI_MODEL"),
			    messages = requestMessages
		    };

		    return await _openAiClient.OpenAiCompletionRequestWithContent(requestContent);
	    } 
	    catch (Exception ex)
	    {
		    _logger.LogError(ex, ex.Message);
		    // Handle exceptions
		    //throw;
	    }

	    return null;
    }

    #endregion
	#region TTS
	
	public async Task<TTSResponse> ConvertSegmentToSpeech(PodcastSegment segment, string voice)
	{
		var responseFilePath = await _openAiClient.OpenAiTTSRequestWithContent(segment.PodcastSegmentId, segment.TextContent, voice);
		
		return new TTSResponse
		{
			AudioId = segment.PodcastSegmentId, // Or a URL if you're serving it via a web server
			AudioUrl = responseFilePath // Or a URL if you're serving it via a web server
		};
	}

	#endregion
}