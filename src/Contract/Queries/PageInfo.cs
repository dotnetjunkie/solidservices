namespace Contract.Queries
{
    /// <summary>Object containing information about paging.</summary>
    public class PageInfo
    {
        /// <summary>The 0-based page index.</summary>
        public int PageIndex { get; set; }

        /// <summary>The number of items in a page.</summary>
        public int PageSize { get; set; } = 20;
    }
}