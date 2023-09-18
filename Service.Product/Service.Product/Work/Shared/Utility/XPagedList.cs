using Newtonsoft.Json;

namespace Service.Product.Shared.Utility
{
    public class PagedList<TEntity> : List<TEntity>
    {
        /// <summary>
        /// Get the current viewing page number.
        /// </summary>
        public int CurrentPage { get; private set; }

        /// <summary>
        /// The number of content to be display in a page.
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Get total number of pages.
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Get total number of contents.
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// Indicate it have a previous page.
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;

        /// <summary>
        /// Indicate it have a next page.
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;

        /// <summary>
        /// Collect paging metadata for 'X-Pagination' Response.Header.
        /// <para>
        ///  { CurrentPage, TotalPages, PageSize, TotalCount, HasPrevious, HasNext }
        /// </para>
        /// </summary>
        public string GetPagingMetadata
        {
            get
            {
                return JsonConvert.SerializeObject(new
                {
                    TotalCount,
                    PageSize,
                    CurrentPage,
                    TotalPages,
                    HasNext,
                    HasPrevious
                });
            }
        }

        public PagedList(List<TEntity> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        /// <summary>
        /// Convert IQueryable&lt;<typeparamref name="TEntity"/>&gt; to PagedList&lt;<typeparamref name="TEntity"/>&gt;.
        /// </summary>
        public static PagedList<TEntity> ToPagedList(IQueryable<TEntity> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<TEntity>(items, count, pageNumber, pageSize);
        }
    }
}

