using System.Linq.Dynamic.Core;
using System.Text;

namespace Auctionify.Core.Persistence.Dynamic
{
    public static class IQueryableDynamicFilterExtensions
    {
        private static readonly string[] _logics = { "and", "or" };

        private static readonly IDictionary<string, string> _operators = new Dictionary<string, string>
        {
            { "eq", "=" },
            { "neq", "!=" },
            { "lt", "<" },
            { "lte", "<=" },
            { "gt", ">" },
            { "gte", ">=" },
            { "isnull", "== null" },
            { "isnotnull", "!= null" },
            { "startswith", "StartsWith" },
            { "endswith", "EndsWith" },
            { "contains", "Contains" },
            { "doesnotcontain", "Contains" }
        };

        public static IQueryable<T> ToDynamic<T>(this IQueryable<T> query, DynamicQuery dynamicQuery)
        {
            if (dynamicQuery.Filter is not null)
                query = Filter(query, dynamicQuery.Filter);

            return query;
        }

        private static IQueryable<T> Filter<T>(IQueryable<T> queryable, Filter filter)
        {
            string where = Transform(filter);
            if (!string.IsNullOrEmpty(where) &&  filter.Value != null)
                queryable = queryable.Where(where, filter.Value);

            return queryable;
        }

        public static string Transform(Filter filter)
        {
            var index = 0;
            if (string.IsNullOrEmpty(filter.Field))
                throw new ArgumentException("Invalid Field");
            if (string.IsNullOrEmpty(filter.Operator) || !_operators.ContainsKey(filter.Operator))
                throw new ArgumentException("Invalid Operator");

            string comparison = _operators[filter.Operator];
            StringBuilder where = new();

            if (!string.IsNullOrEmpty(filter.Value))
            {
                if (filter.Operator == "doesnotcontain")
                    where.Append($"(!np({filter.Field}).{comparison}(@{index.ToString()}))");
                else if (comparison is "StartsWith" or "EndsWith" or "Contains")
                    where.Append($"(np({filter.Field}).{comparison}(@{index.ToString()}))");
                else
                    where.Append($"np({filter.Field}) {comparison} @{index.ToString()}");
            }
            else if (filter.Operator is "isnull" or "isnotnull")
            {
                where.Append($"np({filter.Field}) {comparison}");
            }

            return where.ToString();
        }
    }
}
