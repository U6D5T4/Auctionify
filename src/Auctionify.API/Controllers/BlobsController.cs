using Auctionify.Application.Common.Interfaces;
using Auctionify.Application.Common.Models.Blob;
using Microsoft.AspNetCore.Mvc;

namespace Auctionify.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BlobsController : ControllerBase
	{
		private readonly IBlobService _blobService;

		public BlobsController(IBlobService blobService)
		{
			_blobService = blobService;
		}

		[HttpGet("{name}")]
		public async Task<IActionResult> GetBlob(string name)
		{
			var data = await _blobService.GetBlobAsync(name);
			return File(data.Content, data.ContentType);	
		}

		[HttpGet("list")]
		public async Task<IActionResult> ListBlobs()
		{
			var data = await _blobService.ListBlobsAsync();
			return Ok(data);
		}

		[HttpPost("upload-file")]
		public async Task<IActionResult> UploadFile([FromForm] UploadFileRequest request)
		{
			await _blobService.UploadFileBlobAsync(request.File, request.FilePath);
			return Ok("File uploaded successfully");
		}

		[HttpPost("upload-content")]
		public async Task<IActionResult> UploadContent([FromBody] UploadContentRequest request)
		{
			await _blobService.UploadContentBlobAsync(request.Content, request.FileName);
			return Ok("Content uploaded successfully");
		}

		[HttpDelete("{name}")]
		public async Task<IActionResult> DeleteBlob(string name)
		{
			await _blobService.DeleteBlobAsync(name);
			return Ok("Blob deleted successfully");
		}

		[HttpPost("upload-files")]
		public async Task<IActionResult> UploadFiles([FromForm] UploadFilesRequest request)
		{
			var folderName = Guid.NewGuid().ToString();

			await _blobService.UploadFilesBlobAsync(request.Files, folderName);
			return Ok("Files uploaded successfully");
		}

		[HttpGet("list-folder/{folderName}")]
		public async Task<IActionResult> ListBlobsInFolder(string folderName)
		{
			var data = await _blobService.ListBlobsInFolderAsync(folderName);
			return Ok(data);
		}

		[HttpGet("get-folder/{folderName}/{blobName}")]
		public async Task<IActionResult> GetBlobInFolder(string folderName, string blobName)
		{
			var data = await _blobService.GetBlobInFolderAsync(folderName, blobName);
			return File(data.Content, data.ContentType);
		}

		[HttpDelete("delete-folder/{folderName}")]
		public async Task<IActionResult> DeleteFolder(string folderName)
		{
			await _blobService.DeleteFolderAsync(folderName);
			return Ok("Folder deleted successfully");
		}
	}

	public class UploadFilesRequest
	{
		public IList<IFormFile> Files { get; set; }
	}
}
