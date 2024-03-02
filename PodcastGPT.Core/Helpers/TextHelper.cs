using System.Text;
using System.Text.RegularExpressions;
using PodcastGPT.Data.DTOs;

namespace PodcastGPT.Core.Helpers;

public class TextHelper
{
	public static int EstimateTokenCount(string text)
	{
		// Split the text by word boundaries and punctuation
		var tokens = Regex.Matches(text, @"(\p{L}+)|(\p{N}+)|(\p{P}+)|(\p{Z}+)|(\p{C}+)");
		return tokens.Count;
	}

	public static int EstimateTokenCount(List<OpenAiMessageDto> messages)
	{
		var totalTokenCount = 0;

		foreach (var message in messages)
		{
			var tokenCount = EstimateTokenCount(message.content);
			totalTokenCount += tokenCount;
		}
    
		return totalTokenCount;
	}
	
	public static string TrimContentGarbage(string content)
	{
		content.Replace("\n", " ");
		// Remove excessive whitespaces and newlines
		content = Regex.Replace(content, @"\s+", " ");
		content = Regex.Replace(content, @"(\r\n|\n|\r)", Environment.NewLine);

		return content;
	}
	
	public static string TrimContentToFit(string content, int currentTokenCount, int maxTokens)
	{
		int targetTokenCount = maxTokens - currentTokenCount;
		int contentTokenCount = EstimateTokenCount(content);

		if (contentTokenCount <= targetTokenCount)
		{
			return content;
		}

		int targetByteCount = targetTokenCount * 4;
		int currentByteCount = 0;
		int endIndex = 0;

		// Remove excessive whitespaces and newlines
		content = Regex.Replace(content, @"\s+", " ");
		content = Regex.Replace(content, @"(\r\n|\n|\r)", Environment.NewLine);

		while (currentByteCount < targetByteCount && endIndex < content.Length)
		{
			currentByteCount += Encoding.UTF8.GetByteCount(content[endIndex].ToString());
			endIndex++;
		}

		return content.Substring(0, endIndex);
	}
}