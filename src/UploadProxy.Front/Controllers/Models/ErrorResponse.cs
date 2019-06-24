using Newtonsoft.Json;

namespace UploadProxy.Front.Controllers.Models
{
	public class ErrorResponse
	{
		[JsonProperty(PropertyName = "error")]
		public string Error { get; set; }
	}
}
