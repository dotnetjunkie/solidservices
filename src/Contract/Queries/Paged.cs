namespace Contract.Queries
{
    public class Paged<T>
    {
        public PageInfo Paging { get; set; }

        public T[] Items { get; set; }
    }
}