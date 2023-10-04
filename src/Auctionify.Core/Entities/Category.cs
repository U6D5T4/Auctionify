using Auctionify.Core.Common;

namespace Auctionify.Core.Entities
{
    public class Category : BaseAuditableEntity
    {
        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        public virtual Category ParentCategory { get; set; }

        public virtual ICollection<Category> Children { get; set; }
    }
}
