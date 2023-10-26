namespace Auctionify.Core.Persistence.Dynamic
{
    public class DynamicQuery
    {
        public Filter? Filter { get; set; }

        public DynamicQuery() { }

        public DynamicQuery(Filter? filter)
        {
            Filter = filter;
        }
    }
}
