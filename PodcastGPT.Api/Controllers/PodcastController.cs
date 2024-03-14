using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PodcastGPT.Api.Models;
using PodcastGPT.Core.Models;
using PodcastGPT.Core.Repositories;
using PodcastGPT.Core.Services;
using PodcastGPT.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace PodcastGPT.Api.Controllers;

// [Authorize]
[ApiController]
[Route("api")]
public class PodcastController : ControllerBase
{
	#region Variables
	
	private readonly ILogger<PodcastController> _logger;
	private readonly IGenericRepository<Podcast> _podcastRepository;
	private readonly IGenericRepository<PodcastSegment> _podcastSegmentRepository;
	private readonly IGenericRepository<NewsSiteArticle> _newsSiteArticleRepository;
	private readonly PodcastGenerationService _podcastGenerationService;
	
	#endregion
	#region Initialization

	public PodcastController(
		ILogger<PodcastController> logger,
		IGenericRepository<Podcast> podcastRepository,
		IGenericRepository<PodcastSegment> podcastSegmentRepository,
		IGenericRepository<NewsSiteArticle> newsSiteArticleRepository,
		PodcastGenerationService podcastGenerationService
	)
	{
		_logger = logger;
		_podcastRepository = podcastRepository;
		_podcastSegmentRepository = podcastSegmentRepository;
		_newsSiteArticleRepository = newsSiteArticleRepository;
		_podcastGenerationService = podcastGenerationService;
	}
	
	#endregion

	[HttpGet]
	[Route("podcasts")]
	public async Task<ActionResult<PodcastListResponse>> GetAllPodcasts()
	{
		try
		{
			List<Podcast> podcastsList = await _podcastRepository.GetAllAsync();
			return new PodcastListResponse(podcastsList);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error converting text to speech");
			throw;
		}
	}
	
	[HttpPost]
	[Route("podcasts/generate")]
	public async Task<ActionResult<PodcastDetailResponse>> GeneratePodcast([FromBody] PodcastGenerationRequest podcastGenerationRequest)
	{
		try
		{
			var newPodcast = await _podcastGenerationService.GeneratePodcast(podcastGenerationRequest);
			return new PodcastDetailResponse(newPodcast);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error converting text to speech");
			throw;
		}
	}
	
	[HttpGet]
	[Route("podcasts/{podcastSlug}")]
	public async Task<ActionResult<PodcastDetailResponse>> GetPodcastBySlug([FromRoute] string podcastSlug)
	{
		try
		{
			var allPodcasts = await _podcastRepository.GetAllAsync();
			var podcast = allPodcasts.FirstOrDefault(podcast => podcast.Slug.ToLower().Trim() == podcastSlug.ToLower().Trim());
			return new PodcastDetailResponse(podcast);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error converting text to speech");
			throw;
		}
	}
	
	[HttpDelete]
	[Route("podcasts/{podcastId}")]
	public async Task<ActionResult> DeletePodcastById([FromRoute] Guid podcastId)
	{
		try
		{
			if (podcastId != Guid.Empty)
			{
				var podcast = await _podcastRepository.GetByIdAsync(podcastId);

				foreach (var segment in podcast.PodcastSegments)
				{
					try
					{
						if (!string.IsNullOrWhiteSpace(segment.AudioFileUrl))
						{
							var segmentFile = new FileInfo(segment.AudioFileUrl);
							segmentFile.Delete();
						}
					}
					catch (Exception ex)
					{
						_logger.LogWarning(ex, ex.Message);
					}

					try
					{
						_podcastSegmentRepository.DeleteAsync(segment.PodcastSegmentId);
					}
					catch (Exception ex)
					{
						_logger.LogWarning(ex, ex.Message);
					}
				}

				try
				{
					if (!string.IsNullOrWhiteSpace(podcast.FullAudioFileUrl))
					{
						var fullFile = new FileInfo(podcast.FullAudioFileUrl);
						fullFile.Delete();
					}
				}
				catch (Exception ex)
				{
					_logger.LogWarning(ex, ex.Message);
				}

				foreach (var article in podcast.NewsSiteArticles)
				{
					try
					{
						await _newsSiteArticleRepository.DeleteAsync(article.NewsSiteArticleId);
					}
					catch (Exception ex)
					{
						_logger.LogWarning(ex, ex.Message);
					}
				}

				podcast.PodcastHostPersonaId = Guid.Empty;
				podcast.PodcastGuestPersonaId = Guid.Empty;

				_podcastRepository.SaveChangesAsync();

				await _podcastRepository.DeleteAsync(podcastId);

				return Ok("Podcast deleted successfully");
			} else {
				return BadRequest("A valid PodcastId in Guid format is required");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error converting text to speech");
			throw;
		}
	}
}
