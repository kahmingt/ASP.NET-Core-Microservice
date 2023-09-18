namespace Service.Product.Shared.Utility
{
    /// <summary>
    /// Query parameters to handle Paging, Sorting and Search functionality.
    /// </summary>
    public class QueryableParameterBase
    {
        const int maxPageSize = 50; // Maximum number of details per page
        private int _pageSize = 5;  // Number of details per page

        /// <summary>
        /// Number of content to be display in a page. Max content size is 50.
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

        /// <summary>
        /// Navigate to the page number.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Filter criteria.
        /// </summary>
        public string? FilterBy { get; set; }

        /// <summary>
        /// Order criteria
        /// </summary>
        public string? OrderBy { get; set; }

    }
}