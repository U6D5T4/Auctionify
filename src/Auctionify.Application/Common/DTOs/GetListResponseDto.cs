using Auctionify.Core.Persistence.Paging;

namespace Auctionify.Application.Common.DTOs
{
    public class GetListResponseDto<T> : BasePageableModel
    {
        public IList<T> Items
        {
            get => _items ??= new List<T>();
            set => _items = value;
        }

        private IList<T>? _items;
    }
}
