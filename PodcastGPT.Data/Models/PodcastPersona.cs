namespace PodcastGPT.Data.Models;

public class PodcastPersona
{
	public Guid PodcastPersonaId { get; set; }
	public string Name { get; set; }
	public string VoiceId { get; set; }
	public string Type { get; set; }
	
	//public List<PodcastSegment> PodcastSegments { get; set; }
	
	public List<Podcast> Podcasts { get; set; }
	// public Guid PodcastId { get; set; }
}
