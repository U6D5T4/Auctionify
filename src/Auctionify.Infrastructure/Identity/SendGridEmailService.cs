using SendGrid.Helpers.Mail;
using SendGrid;
using Auctionify.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Auctionify.Infrastructure.Identity
{
	public class SendGridEmailService : IEmailService
	{
		private readonly IConfiguration _configuration;

		public SendGridEmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string content)
		{
			var apiKey = _configuration["EmailSender:SendGridEmail"];
			var client = new SendGridClient(apiKey);
			var from = new EmailAddress(_configuration["EmailSender"], "Auctionify");
			var to = new EmailAddress(toEmail);
			var htmlContent = $"<strong>{content}</strong>";
			var msg = MailHelper.CreateSingleEmail(from, to, subject, content, htmlContent);
			var response = await client.SendEmailAsync(msg);
		}
	}
}
