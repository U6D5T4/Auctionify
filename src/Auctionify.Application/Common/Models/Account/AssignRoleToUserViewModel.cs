using Auctionify.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Auctionify.Application.Common.Models.Account
{
    public class AssignRoleToUserViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        [Required]
        public Role Role { get; set; }
    }
}
