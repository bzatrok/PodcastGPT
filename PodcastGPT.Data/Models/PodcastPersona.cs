namespace PodcastGPT.Data.Models;

public class PodcastPersona
{
	public Guid PodcastPersonaId { get; set; }
	public string Name { get; set; }
	public string VoiceId { get; set; }
	public string Type { get; set; }

	public virtual ICollection<Podcast> GuestOnPodcasts { get; set; } = new List<Podcast>();
	public virtual ICollection<Podcast> HostOnPodcasts { get; set; } = new List<Podcast>();
}