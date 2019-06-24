using System.Threading.Tasks;

namespace UploadProxy.Core.Services
{
	public interface IEmailSender
	{
		Task SendEmailAsync(string email, string subject, string message);
		Task SendEmailConfirmationAsync(string email, string callbackUrl);
		Task SendResetPasswordAsync(string email, string callbackUrl);
	}
}
