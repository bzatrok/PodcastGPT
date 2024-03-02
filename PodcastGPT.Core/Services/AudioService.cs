using System.Net.Http.Headers;
using PodcastGPT.Core.Helpers;
using PodcastGPT.Core.Repositories;
using PodcastGPT.Data.Models;
using Microsoft.Extensions.Logging;

namespace PodcastGPT.Core.Services;

public class AudioService
{
	#region Variables

	private readonly ILogger<AudioService> _logger;
	private readonly IGenericRepository<Podcast> _podcastRepository;
	private readonly AudioFileHelper _audioFileHelper;


	#endregion
	#region Initialization

	public AudioService(
		ILogger<AudioService> logger,
		IGenericRepository<Podcast> podcastRepository,
		AudioFileHelper audioFileHelper
		)
	{
		_logger = logger;
		_podcastRepository = podcastRepository;
		_audioFileHelper = audioFileHelper;
	}
	
	#endregion
	#region Requests
	
	public async Task<string> MergeAudioFilesForPodcast(Podcast podcast)
	{
		var outputPath = $"generated/podcasts/{podcast.Slug}.mp3";	
		var audioFilePaths = podcast.PodcastSegments.Select(segment => segment.AudioFileUrl).ToList();
		
		var mergedFilePath = await _audioFileHelper.MergeAudioFiles(
			outputPath,
			audioFilePaths);
		
		foreach (var audioFilePath in audioFilePaths)
		{
			try
			{
				var file = new FileInfo(audioFilePath);
				file.Delete();
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Error deleting audio file");
			}
		}

		return mergedFilePath;
	}

	public async Task<HttpResponseMessage> StreamPodcastById(string audioId)
	{
		if (!string.IsNullOrWhiteSpace(audioId))
		{
			var podcast = await _podcastRepository.GetByIdAsync(new Guid(audioId));
			
			var streamPath = podcast.FullAudioFileUrl;
			var fileStream = new FileStream(streamPath, FileMode.Open, FileAccess.Read, FileShare.Read, 32768, true);
			var response = new HttpResponseMessage
			{
				Content = new StreamContent(fileStream)
			};
			response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
			return response;
		}

		throw new FileNotFoundException();
	}
	
	#endregion
}