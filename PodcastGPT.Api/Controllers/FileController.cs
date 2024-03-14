using PodcastGPT.Core.Services;
using Microsoft.AspNetCore.Mvc;
using PodcastGPT.Core.Repositories;
using PodcastGPT.Data.Models;

namespace PodcastGPT.Api.Controllers;

// [Authorize]
[ApiController]
[Route("api")]
public class FileController : ControllerBase
{
	#region Variables
	
	private readonly ILogger<FileController> _logger;
	private readonly IGenericRepository<Podcast> _podcastReposity;
	
	#endregion
	#region Initialization

	public FileController(
		ILogger<FileController> logger,
		IGenericRepository<Podcast> podcastReposity
		)
	{
		_logger = logger;
		_podcastReposity = podcastReposity;
	}
	
	#endregion

	[HttpGet]
	[Route("file/podcast-audio/{podcastId}")]
	public async Task<IActionResult> StreamAudio([FromRoute] string podcastId)
	{
		try
		{
			var podcast = await _podcastReposity.GetByIdAsync(new Guid(podcastId));
			
			if (podcast is null)
				return NotFound();
			
			// var fileStream = await _fileService.GetAudioFileStreamForFilePath(podcast.FullAudioFileUrl);
			//
			// if(fileStream == null)
			// 	return NotFound(); 

			return PhysicalFile(podcast.FullAudioFileUrl, "audio/mpeg", $"{podcast.Slug}.mp3"); // returns a FileStreamResult
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error converting text to speech");
			throw;
		}
	}
}
