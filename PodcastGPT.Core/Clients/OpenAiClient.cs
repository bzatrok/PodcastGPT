using PodcastGPT.Core.Models.OpenAi;
using PodcastGPT.Data.DTOs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PodcastGPT.Core.Clients;

public class OpenAiClient
{
	private readonly ILogger<OpenAiClient> _logger;
	
	public OpenAiClient(ILogger<OpenAiClient> logger)
	{
		_logger = logger;
	}
	
	public async Task<string> OpenAiCompletionRequestWithContent(OpenAiRequestDto requestContent)
	{
		var client = new HttpClient();
		client.Timeout = TimeSpan.FromSeconds(60);

		var apiKey = Environment.GetEnvironmentVariable("OPEN_AI_API_KEY");
		var baseUrl = "https://api.openai.com";
		var requestUri = new Uri($"{baseUrl}/v1/chat/completions");

		var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri);
		httpRequest.Headers.Add("Authorization", $"Bearer {apiKey}");
		httpRequest.Content = new StringContent(JsonConvert.SerializeObject(requestContent), System.Text.Encoding.UTF8, "application/json");

		var response = await client.SendAsync(httpRequest);
		var responseContent = await response.Content.ReadAsStringAsync();

		var result = JsonConvert.DeserializeObject<OpenAiChatResponse>(responseContent);

		var aiResponse = result.choices[0].message.content;

		return aiResponse;
	}
	
	public async Task<string> OpenAiTTSRequestWithContent(Guid id, string text, string voice)
	{
		var requestContent = new
		{
			model = "tts-1",
			input = text,
			voice = voice
		};
		
		try
		{
			var apiKey = Environment.GetEnvironmentVariable("OPEN_AI_API_KEY");
			var baseUrl = "https://api.openai.com";
			var requestUri = new Uri($"{baseUrl}/v1/audio/speech");

			var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri);
			httpRequest.Headers.Add("Authorization", $"Bearer {apiKey}");
			httpRequest.Content = new StringContent(JsonConvert.SerializeObject(requestContent), System.Text.Encoding.UTF8, "application/json");
			
			var client = new HttpClient();
			var response = await client.SendAsync(httpRequest);

			var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"generated/{id.ToString()}.mp3"); // Update this path and filename as needed
			var fileInfo = new FileInfo(filePath);
			Directory.CreateDirectory(fileInfo.Directory.FullName);
			
			using (var httpStream = await response.Content.ReadAsStreamAsync())
			{
				using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
				{
					await httpStream.CopyToAsync(fileStream);
				}
			}
			
			if (!fileInfo.Exists)
				throw new Exception("File not found");

			// var audioResponse = new TTSResponse
			// {
			// 	AudioId = id, // Or a URL if you're serving it via a web server
			// 	AudioUrl = filePath // Or a URL if you're serving it via a web server
			// };

			return filePath;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, ex.Message);
			// Handle exceptions
			throw;
		}
	}
	
	public async Task<string> GenerateSingleAssistantResponse(string systemPrompt, string request)
	{
		try
		{
			var requestContent = new OpenAiRequestDto
			{
				model = Environment.GetEnvironmentVariable("OPEN_AI_MODEL"),
				messages = new List<OpenAiMessageDto>
				{
					new OpenAiMessageDto()
					{
						role = "system",
						content = systemPrompt
					},
					new OpenAiMessageDto()
					{
						role = "user",
						content = request
					}
				}
			};
		    
			return await OpenAiCompletionRequestWithContent(requestContent);
		} 
		catch (Exception ex)
		{
			_logger.LogError(ex, ex.Message);
			// Handle exceptions
			//throw;
		}

		return null;
	}
}