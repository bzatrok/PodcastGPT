namespace PodcastGPT.Core.Constants;

public class RedisKeys
{
	public static string AudioCacheKey(string audioId)
	{
		return $"PodcastGPT:AudioCache:{audioId}";
	}
}