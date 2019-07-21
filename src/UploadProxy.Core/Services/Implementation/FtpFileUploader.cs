using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

				var credentials = new NetworkCredential(username.Normalize(), password.Normalize());
				var directory = await CreateDirectory(ftpPath, credentials);
				var filePath = ftpPath.EndsWith('/')
					? $"{ftpPath}{directory}/{filename}"
					: $"{ftpPath}/{directory}/{filename}";

				var request = (FtpWebRequest)WebRequest.Create(filePath);
				request.Credentials = credentials;
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
					? $"{httpPath}{directory}/{filename}"
					: $"{httpPath}/{directory}/{filename}";

				_logger.LogInformation($"Uploaded file: '{filePath}' ('{httpPath}')");
				return httpPath;
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
				return string.Empty;
			}
		}

		private async Task<string> CreateDirectory(string ftpPath, ICredentials credentials)
		{
			for (var i = 0; i < 10; i++)
			{
				try
				{
					var directory = string.Join("",
						Guid.NewGuid().ToByteArray().Take(8).Select(e => $"{e:X2}"));
					var directoryPath = ftpPath.EndsWith('/')
						? $"{ftpPath}{directory}"
						: $"{ftpPath}/{directory}";
					var request = (FtpWebRequest)WebRequest.Create(directoryPath);
					request.Credentials = credentials;
					request.Method = WebRequestMethods.Ftp.MakeDirectory;
					request.EnableSsl = true;
					request.UsePassive = true;
					await request.GetResponseAsync();

					return directory;
				}
				catch (Exception e)
				{
					_logger.LogError(e.Message);
				}
			}

			_logger.LogError("Cannot create ftp folder");
			throw new ArgumentException("Cannot create ftp folder");
		}
	}
}
