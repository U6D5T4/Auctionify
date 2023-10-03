using Auctionify.Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auctionify.Core.Entities
{
    internal class User : BaseAuditableEntity
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        public string? UserName { get; set; }

        public string? NormalizedUserName { get; set; }

        public string SecurityStamp { get; set; }

        public bool EmailConfirmed { get; set; }

        [MaxLength(128)]
        public string PasswordHash { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        [MaxLength(11)]
        public int? PhoneNumber { get ; set; }
    }
}
