namespace Auctionify.Application.Common.Interfaces
{
	public interface IEmailService
	{
		Task SendEmailAsync(string toEmail, string subject, string content);
	}
}
