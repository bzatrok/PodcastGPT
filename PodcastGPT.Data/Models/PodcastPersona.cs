namespace PodcastGPT.Data.Models;

public class PodcastPersona
{
	public Guid PodcastPersonaId { get; set; }
	public string Name { get; set; }
	public string VoiceId { get; set; }
	public string Type { get; set; }
	
	//public List<PodcastSegment> PodcastSegments { get; set; }
	
	public virtual ICollection<Podcast> Podcasts { get; set; } = new List<Podcast>();
	// public Guid PodcastId { get; set; }
}
