namespace PodcastGPT.Data.Models;

public class OpenAiConversation
{
	public Guid OpenAiConversationId { get; set; }
	
	public List<OpenAiMessage> OpenAiMessages { get; set; }
}