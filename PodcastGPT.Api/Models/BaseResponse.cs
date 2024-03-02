namespace PodcastGPT.Api.Models;

public class BaseResponse
{
	public bool Success { get; set; }
	public int StatusCode { get; set; }
	public string Message { get; set; }
	public Exception Exception { get; set; }
	
	public BaseResponse()
	{
		Success = true;
		StatusCode = 200;
		Message = "OK";
	}
}