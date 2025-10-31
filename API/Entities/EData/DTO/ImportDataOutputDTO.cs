using AlpimiAPI.Responses;

namespace AlpimiAPI.Entities.EData.DTO
{
    public class ImportDataOutputDTO
    {
        public required int successfulItems { get; set; }

        public required Dictionary<string, List<UnsuccsefulItem>> unsuccessfulItems { get; set; }
    }

    public class UnsuccsefulItem
    {
        public required IEnumerable<ErrorObject> Reason { get; set; }

        public required int RowIndex { get; set; }
    }
}
