namespace BusinessLayer
{
    using System.Collections.Generic;
    using System.Linq;
    using Contract.Queries;

    public static class PagingExtensions
    {
        /// <summary>Apply paging in memory, using LINQ to Objects.</summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        /// <param name="collection">The collection</param>
        /// <param name="paging">The optional paging object. When null, the default paging values will be used.</param>
        /// <returns>A paged result.</returns>
        public static Paged<T> Page<T>(this IEnumerable<T> collection, PageInfo paging)
        {
            paging = paging ?? new PageInfo();

            IEnumerable<T> items = paging.IsSinglePage()
                ? collection
                : collection.Skip(paging.PageIndex * paging.PageSize).Take(paging.PageSize);

            return new Paged<T> { Items = items.ToArray(), Paging = paging };
        }

        /// <summary>Apply paging using an efficient database query.</summary>
        /// <typeparam name="T">The type of objects to enumerate.</typeparam>
        /// <param name="collection">The collection</param>
        /// <param name="paging">The optional paging object. When null, the default paging values will be used.</param>
        /// <returns>A paged result.</returns>
        public static Paged<T> Page<T>(this IQueryable<T> collection, PageInfo paging)
        {
            paging = paging ?? new PageInfo();

            IQueryable<T> items = paging.IsSinglePage()
                ? collection
                : collection.Skip(paging.PageIndex * paging.PageSize).Take(paging.PageSize);

            return new Paged<T> { Items = items.ToArray(), Paging = paging };
        }
    }
}