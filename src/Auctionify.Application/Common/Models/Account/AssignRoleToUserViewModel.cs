using Auctionify.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Auctionify.Application.Common.Models.Account
{
    public class AssignRoleToUserViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public Role Role { get; set; }
    }
}
