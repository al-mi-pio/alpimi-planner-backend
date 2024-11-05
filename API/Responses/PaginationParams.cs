using AlpimiAPI.Settings;
using alpimi_planner_backend.API.Settings;

namespace AlpimiAPI.Responses
{
    public class PaginationParams
    {
        public int PerPage { get; set; }
        public int Offset { get; set; }
        public string SortBy { get; set; }
        public string SortOrder { get; set; }

        public PaginationParams(int? perPage, int? offset, string? sortBy, string? sortOrder)
        {
            PerPage = perPage ?? 20;

            Offset = offset ?? (PaginationSettings.page - 1) * PerPage;
            SortBy = sortBy ?? PaginationSettings.sortBy;
            SortOrder = sortOrder ?? PaginationSettings.sortOrder;
        }
    }
}
