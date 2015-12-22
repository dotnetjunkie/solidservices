using System.Collections.Generic;
using System.Linq;
using Contract.Queries;

namespace BusinessLayer
{
    public static class PagingExtensions
    {
        public static Paged<T> Page<T>(this IEnumerable<T> collection, PageInfo paging) => new Paged<T>
        {
            Items = collection.Skip(paging.PageIndex * paging.PageSize).Take(paging.PageSize).ToArray(),
            Paging = paging,
        };

        public static Paged<T> Page<T>(this IQueryable<T> collection, PageInfo paging) => new Paged<T>
        {
            Items = collection.Skip(paging.PageIndex * paging.PageSize).Take(paging.PageSize).ToArray(),
            Paging = paging,
        };
    }
}