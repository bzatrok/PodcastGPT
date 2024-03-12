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
	private readonly IGenericRepository<PodcastSegment> _podcastSegmentRepository;
	private readonly IGenericRepository<PodcastPersona> _podcastPersonaRepository;
	private readonly AudioService _audioService;
	private readonly OpenAiClient _openAiClient;

	#endregion
	#region Initialization

	public OpenAIService(
		ILogger<OpenAIService> logger,
		IGenericRepository<Podcast> podcastRepository,
		IGenericRepository<PodcastSegment> podcastSegmentRepository,
		IGenericRepository<PodcastPersona> podcastPersonaRepository,
		AudioService audioService,
		OpenAiClient openAiClient
	)
	{
		_logger = logger;
		_podcastRepository = podcastRepository;
		_podcastSegmentRepository = podcastSegmentRepository;
		_podcastPersonaRepository = podcastPersonaRepository;
		_audioService = audioService;
		_openAiClient = openAiClient;
	}
	
	#endregion
	#region Create Podcast
	
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

		// List<PodcastPersona> personas = await _podcastPersonaRepository.GetAllAsync();
		// 	
		// PodcastPersona? hostPersona = personas.FirstOrDefault(persona => persona.Type == "interviewer");
		// PodcastPersona? guestPersona = personas.FirstOrDefault(persona => persona.Type == "guest");

		var hostPersona = podcast.PodcastHostPersona;
		var guestPersona = podcast.PodcastGuestPersona;
		
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
			
			var nextSegment = await GenerateAssistantResponseForPodcastSegments(systemMessage, segments);

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
			var generatedAudioForSegment = await ConvertSegmentToSpeech(segment, persona.VoiceId);
			segment.AudioFileUrl = generatedAudioForSegment.AudioUrl;
			await _podcastSegmentRepository.SaveChangesAsync();
		}
		
		podcast.Status = "audiosegmentsready";
		
		await _podcastRepository.SaveChangesAsync();

		var mergedFilePath = await _audioService.MergeAudioFilesForPodcast(podcast);
		
		podcast.FullAudioFileUrl = mergedFilePath;

		podcast.Status = "ready";
		await _podcastRepository.SaveChangesAsync();
		
		return podcast;
	}
    
    public async Task<string> GenerateAssistantResponseForPodcastSegments(string systemPrompt, IEnumerable<PodcastSegment> messages)
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
    #region Re-generate Audio for Podcast

    // public async Task<Podcast> RegenerateAudioForPodcastId(Guid podcastId)
    // {
	   //
    // }

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