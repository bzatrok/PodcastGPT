using System.Net.Http.Headers;
using System.Text;
using PodcastGPT.Core.Constants;
using PodcastGPT.Core.Helpers;
using PodcastGPT.Core.Models.TextToSpeech;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PodcastGPT.Core.Services;

public class ElevenlabsService
{
	#region Variables

	private readonly ILogger<ElevenlabsService> _logger;
	private readonly HttpClient _httpClient;
	private readonly RedisHelper _redisHelper;

	#endregion
	#region Initialization

	public ElevenlabsService(
		ILogger<ElevenlabsService> logger,
		HttpClient httpClient,
		RedisHelper redisHelper
		)
	{
		_logger = logger;
		_httpClient = httpClient;
		_redisHelper = redisHelper;
	}
	
	#endregion
	#region Requests

	public async Task<TTSResponse> ConvertTextToSpeech(TTSRequest request)
	{
		var ttsRequest = new ElevenlabsRequest()
		{
			ModelId = "eleven_multilingual_v1",
			Text = request.Text,
			VoiceSettings = new VoiceSettings
			{
				SimilarityBoost = 0.5,
				Stability = 0.5,
			}
		};

		var voiceId = "aoj7mycNu4l3DoxWHEER"; // HAL
		// var voiceId = "aoj7mycNu4l3DoxWHEER"; // Radio guy
		
		try
		{
			HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}");
			requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("audio/mpeg"));
			requestMessage.Headers.Add("xi-api-key", Environment.GetEnvironmentVariable("ELEVENLABS_API_KEY"));
			
			var requestJson = JsonConvert.SerializeObject(ttsRequest, new JsonSerializerSettings
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				DefaultValueHandling = DefaultValueHandling.Ignore
			});
			requestMessage.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
			var httpResponse = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead); // Stream the response

			if (httpResponse.IsSuccessStatusCode)
			{				
				var audioId = Guid.NewGuid();

				var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"generated/{audioId}.mpga"); // Update this path and filename as needed
				var fileInfo = new FileInfo(filePath);
				Directory.CreateDirectory(fileInfo.Directory.FullName);
				
				using (var httpStream = await httpResponse.Content.ReadAsStreamAsync())
				{
					using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
					{
						await httpStream.CopyToAsync(fileStream);
					}
				}
				
				if (!fileInfo.Exists)
					throw new Exception("File not found");

				var audioResponse = new TTSResponse
				{
					AudioId = audioId, // Or a URL if you're serving it via a web server
					AudioUrl = filePath // Or a URL if you're serving it via a web server
				};

				string cacheKey = RedisKeys.AudioCacheKey(audioId.ToString());

				_redisHelper.SetObject(cacheKey, audioResponse);
				
				return audioResponse;
			}
			else
			{
				var responseJson = await httpResponse.Content.ReadAsStringAsync();
				var ttsResponse = JsonConvert.DeserializeObject<object>(responseJson);
				throw new Exception("422 received");
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, ex.Message);
			// Handle exceptions
			throw;
		}

		return null;
	}
	
	#endregion
}