using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace UploadProxy.Core.Services.Implementation
{
	public class FtpFileUploader : IFileUploader
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<FtpFileUploader> _logger;

		public FtpFileUploader(IConfiguration configuration, ILogger<FtpFileUploader> logger)
		{
			_configuration = configuration;
			_logger = logger;
		}

		public async Task<string> Upload(string filename, Stream stream)
		{
			try
			{
				var ftpPath = _configuration["FtpPath"];
				var username = _configuration["FtpUsername"];
				var password = _configuration["FtpPassword"];
				var httpPath = _configuration["HttpPath"];

				filename = $"{Guid.NewGuid()}-{filename}";
				ftpPath = ftpPath.EndsWith('/')
					? $"{ftpPath}{filename}"
					: $"{ftpPath}/{filename}";

				var request = (FtpWebRequest)WebRequest.Create(ftpPath);
				request.Credentials = new NetworkCredential(username.Normalize(), password.Normalize());
				request.Method = WebRequestMethods.Ftp.UploadFile;
				request.EnableSsl = true;
				request.UsePassive = true;

				using (stream)
				{
					using (var ftpStream = await request.GetRequestStreamAsync())
					{
						await stream.CopyToAsync(ftpStream);
					}
				}

				httpPath = httpPath.EndsWith('/')
					? $"{httpPath}{filename}"
					: $"{httpPath}/{filename}";

				_logger.LogInformation($"Uploaded file: '{ftpPath}' ('{httpPath}')");
				return httpPath;
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
				return string.Empty;
			}
		}
	}
}
