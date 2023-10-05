using System.ComponentModel.DataAnnotations;

namespace Auctionify.Application.Common.Models
{
    /// <summary>
    /// View model for auth flow
    /// </summary>
    public class LoginViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
