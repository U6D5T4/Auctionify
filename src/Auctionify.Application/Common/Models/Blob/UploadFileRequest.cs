using Microsoft.AspNetCore.Http;

namespace Auctionify.Application.Common.Models.Blob
{
	public class UploadFileRequest
	{
		public IFormFile File { get; set; }
		public string FilePath { get; set; }
	}
}
