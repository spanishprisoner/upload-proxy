using Newtonsoft.Json;

namespace UploadProxy.Front.Controllers.Models
{
	public class RefreshTokenResponse
	{
		[JsonProperty(PropertyName = "refreshToken")]
		public string RefreshToken { get; set; }
	}
}
