namespace AlpimiAPI.Responses
{
    public class Pagination
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int Page { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }

        public Pagination(
            int totalItems,
            int itemsPerPage,
            int page,
            string sortBy,
            string sortOrder
        )
        {
            TotalItems = totalItems;
            ItemsPerPage = itemsPerPage;
            Page = page;
            SortBy = sortBy;
            SortOrder = sortOrder;
        }
    }
}
