using System;
using System.Net.Http;
using System.Threading.Tasks;
using PodcastGPT.Core.Models.Stream;
using PodcastGPT.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PodcastGPT.Api.Controllers;

// [Authorize]
[ApiController]
[Route("api")]
public class AudioStreamController : ControllerBase
{
	#region Variables
	
	private readonly ILogger<AudioStreamController> _logger;
	private readonly AudioService _AudioService;
	
	#endregion
	#region Initialization

	public AudioStreamController(
		ILogger<AudioStreamController> logger,
		AudioService AudioService
		)
	{
		_logger = logger;
		_AudioService = AudioService;
	}
	
	#endregion

	[HttpGet]
	[Route("stream/{audioId}")]
	public async Task<HttpResponseMessage> StreamAudio([FromRoute] string audioId)
	{
		try
		{
			return await _AudioService.StreamPodcastById(audioId);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error converting text to speech");
			throw;
		}
	}
}
