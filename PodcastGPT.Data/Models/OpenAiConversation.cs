namespace PodcastGPT.Data.Models;

public class OpenAiConversation
{
	public Guid OpenAiConversationId { get; set; }
	
	public virtual ICollection<OpenAiMessage> OpenAiMessages { get; set; }	= new List<OpenAiMessage>();
}