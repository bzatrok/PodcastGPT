namespace PodcastGPT.Data.Models;

public class OpenAiMessage
{
	public Guid OpenAiMessageId { get; set; }
	public string Role { get; set; }
	public string Content { get; set; }
	public DateTime Time { get; set; }
	public string messageType { get; set; }
	
	public virtual OpenAiConversation OpenAiConversation { get; set; }
	public Guid OpenAiConversationId { get; set; }
}
