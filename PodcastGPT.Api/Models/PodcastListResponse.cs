using System.Collections.Generic;
using PodcastGPT.Data.Models;

namespace PodcastGPT.Api.Models;

public class PodcastListResponse : BaseResponse
{
	public List<Podcast> Podcasts { get; set; }
	
	public PodcastListResponse(List<Podcast> podcasts)
	{
		Podcasts = podcasts;
	}
}