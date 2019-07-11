using System.IO;
using System.Threading.Tasks;

namespace UploadProxy.Core.Services
{
	public interface IFileUploader
	{
		Task<string> Upload(string filename, Stream stream);
	}
}
