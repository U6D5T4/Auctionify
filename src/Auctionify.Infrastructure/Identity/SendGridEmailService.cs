using SendGrid.Helpers.Mail;
using SendGrid;
using Auctionify.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Auctionify.Infrastructure.Identity
{
	public class SendGridEmailService : IEmailService
	{
		private readonly IConfiguration configuration;

		public SendGridEmailService(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string content)
		{
            var apiKey = configuration["EmailSender:SendGridEmail"];
			var client = new SendGridClient(apiKey);
			var from = new EmailAddress(configuration["EmailSender"], "Auctionify");
			var to = new EmailAddress(toEmail);
			var htmlContent = $"<strong>{content}</strong>";
			var msg = MailHelper.CreateSingleEmail(from, to, subject, content, htmlContent);
			var response = await client.SendEmailAsync(msg);
		}
	}
}
