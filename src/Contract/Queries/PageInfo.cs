namespace Contract.Queries
{
    /// <summary>Object containing information about paging.</summary>
    public class PageInfo
    {
        /// <summary>Returns a PageInfo that represents the request for a single page.</summary>
        public static PageInfo SinglePage() => new PageInfo { PageIndex = 0, PageSize = -1 };

        /// <summary>The 0-based page index.</summary>
        public int PageIndex { get; set; }

        /// <summary>The number of items in a page.</summary>
        public int PageSize { get; set; } = 20;

        /// <summary>Gets the value indicating whether the page info represents the request for a single page.</summary>
        public bool IsSinglePage() => this.PageIndex == 0 && this.PageSize == -1;
    }
}