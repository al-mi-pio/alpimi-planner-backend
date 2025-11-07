namespace alpimi_planner_backend.Collisions.CollisionDataObjects
{
    public class CollisionScanInfo
    {
        public Guid historyEntryId { get; set; }
        public string scanType { get; set; }
        public Guid? scanBlockId { get; set; }
        public DateOnly? scanDate { get; set; }

        public CollisionScanInfo(
            Guid historyEntryId,
            string scanType,
            Guid? scanBlockId,
            DateOnly? scanDate
        )
        {
            this.historyEntryId = historyEntryId;
            this.scanType = scanType;
            this.scanBlockId = scanBlockId;
            this.scanDate = scanDate;
        }
    }
}
