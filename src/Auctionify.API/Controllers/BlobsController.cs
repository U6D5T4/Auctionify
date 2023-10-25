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
		public async Task<IActionResult> UploadFile([FromBody] UploadFileRequest request)
		{
			await _blobService.UploadFileBlobAsync(request.FilePath, request.FileName);
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
	}
}
