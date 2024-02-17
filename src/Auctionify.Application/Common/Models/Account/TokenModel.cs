namespace Auctionify.Application.Common.Models.Account
{
	/// <summary>
	/// Token model
	/// </summary>
	public class TokenModel
	{
		public string AccessToken { get; set; }

		public DateTime? ExpireDate { get; set; }

		public string Role { get; set; }

		public int UserId { get; set; }

		public List<string> Roles { get; set; }
	}
}
