namespace PodcastGPT.Core.Helpers;

public class AudioFileHelper
{
	public async Task<string> MergeAudioFiles(string outputPath, List<string> filePathsToMerge)
	{
		var mergedFilePath = Path.Combine(Directory.GetCurrentDirectory(), outputPath);

		// Ensure the directory for the merged file exists
		Directory.CreateDirectory(Path.GetDirectoryName(mergedFilePath));

		await using (var mergedFileStream = new FileStream(mergedFilePath, FileMode.Create))
		{
			foreach (var filePath in filePathsToMerge)
			{
				var mp3Bytes = await File.ReadAllBytesAsync(filePath);
				// Simple check to skip ID3 tag if present
				var startOffset = mp3Bytes.Take(3).SequenceEqual(new byte[] { 0x49, 0x44, 0x33 }) ? SkipId3Tag(mp3Bytes) : 0;
				await mergedFileStream.WriteAsync(mp3Bytes, startOffset, mp3Bytes.Length - startOffset);
			}
		}

		return mergedFilePath;
	}

	private int SkipId3Tag(byte[] mp3Bytes)
	{
		// Assuming ID3v2 tag header is present, skip the header and return the offset
		// ID3v2 tag size is in bytes 6 to 9, but size is encoded as syncsafe integer
		int size = (mp3Bytes[6] << 21) | (mp3Bytes[7] << 14) | (mp3Bytes[8] << 7) | mp3Bytes[9];
		return 10 + size; // Header (10 bytes) + size
	}
}