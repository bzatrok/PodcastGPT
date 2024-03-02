namespace PodcastGPT.Data.Models;

public class PodcastSegment
{
	public Guid PodcastSegmentId { get; set; }
	public int Order { get; set; }
	public string TextContent { get; set; }
	public string? AudioFileUrl { get; set; }
	public string OpenAiRole { get; set; }

	public virtual Podcast Podcast { get; set; }
	public Guid PodcastId { get; set; }
	
	public virtual PodcastPersona PodcastPersona { get; set; } 
	public Guid PodcastPersonaId { get; set; }
}