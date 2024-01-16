namespace Auctionify.Core.Persistence.Paging
{
	public static class IEnumerablePaginateExtensions
	{
		public static Paginate<T> Paginate<T>(
			this IEnumerable<T> source,
			int pageIndex,
			int pageSize
		)
		{
			var count = source.Count();
			var items = source.Skip(pageIndex * pageSize).Take(pageSize).ToList();

			return new Paginate<T>
			{
				Index = pageIndex,
				Size = pageSize,
				From = 0,
				Count = count,
				Items = items,
				Pages = (int)Math.Ceiling(count / (double)pageSize),
			};
		}
	}
}
