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