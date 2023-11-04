namespace Auctionify.Infrastructure.Common.Options
{
	public class AuthSettingsOptions
	{
		public const string AuthSettings = "AuthSettings";
		public string Key { get; set; }
		public string Issuer { get; set; }
		public string Audience { get; set; }
		public int AccessTokenExpirationInMinutes { get; set; }
		public int RefreshTokenExpirationInDays { get; set; }
	}
}
