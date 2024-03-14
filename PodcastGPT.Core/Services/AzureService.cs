using Microsoft.Extensions.Logging;

namespace PodcastGPT.Core.Services;

public class AzureService
{
	#region Variables

	private readonly ILogger<AzureService> _logger;


	#endregion
	#region Initialization

	public AzureService(
		ILogger<AzureService> logger
	)
	{
		_logger = logger;
	}
	
	#endregion
	#region Storage Account
	
	// private async Task EnsuretorageAccount()
	// {
	// 	// Create a new storage account
	// 	// var storageAccount = new StorageAccount();
	// 	// storageAccount.Create();
	// }
	//
	// public async Task UploadBlobToStorageAccount(string filePath)
	// {
	// 	// Upload a blob
	// 	// var blob = new Blob();
	// 	// blob.Upload();
	// 	return Task.CompletedTask;
	// }
	
	#endregion
}