namespace Auctionify.Application.Common.Interfaces
{
	public interface IEmailService
	{
		public Task SendEmailAsync(string toEmail, string subject, string content);
	}
}
