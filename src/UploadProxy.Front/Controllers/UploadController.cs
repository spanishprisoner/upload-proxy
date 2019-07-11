using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UploadProxy.Core.Services;

namespace UploadProxy.Front.Controllers
{
	[Route("api/upload")]
	[Authorize]
	public class UploadController : Controller
	{
		private readonly IFileUploader _fileUploader;

		public UploadController(IFileUploader fileUploader)
		{
			_fileUploader = fileUploader;
		}

		[HttpPost]
		public async Task<IActionResult> Upload([FromForm] IFormFile file)
		{
			var filename = await _fileUploader.Upload(file.FileName, file.OpenReadStream());

			return string.IsNullOrWhiteSpace(filename)
				? throw new Exception("File upload failed")
				: Ok(filename);
		}
	}
}
