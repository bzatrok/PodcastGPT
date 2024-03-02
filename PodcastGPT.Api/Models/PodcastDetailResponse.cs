using PodcastGPT.Data.Models;

namespace PodcastGPT.Api.Models;

public class PodcastDetailResponse : BaseResponse
{
	public Podcast Podcast { get; set; }
	
	public PodcastDetailResponse(Podcast podcast)
	{
		Podcast = podcast;
	}
}