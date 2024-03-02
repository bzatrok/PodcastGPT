using PodcastGPT.Core.Models.TextToSpeech;
using PodcastGPT.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PodcastGPT.Api.Controllers;

// [Authorize]
[ApiController]
[Route("api")]
public class TextToSpeechController : ControllerBase
{
	#region Variables
	
	private readonly ILogger<TextToSpeechController> _logger;
	private readonly ElevenlabsService _elevenlabsService;
	
	#endregion
	#region Initialization

	public TextToSpeechController(
		ILogger<TextToSpeechController> logger,
		ElevenlabsService elevenlabsService
		)
	{
		_logger = logger;
		_elevenlabsService = elevenlabsService;
	}
	
	#endregion

	// [HttpPost]
	// [Route("tts/convert")]
	// public async Task<ActionResult<TTSResponse>> ConvertTextToSpeech([FromBody] TTSRequest request)
	// {
	// 	try
	// 	{
	// 		return await _elevenlabsService.ConvertTextToSpeech(request);
	// 	}
	// 	catch (Exception ex)
	// 	{
	// 		_logger.LogError(ex, "Error converting text to speech");
	// 		throw;
	// 	}
	// }
}
