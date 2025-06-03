namespace AlpimiAPI.Entities.EHistory.DTO
{
    public class AddToHistoryDTO
    {
        public Guid Id { get; set; }

        public required DateTime Timestamp { get; set; }

        public required Guid AffectedEntityId { get; set; }

        public required String AffectedEntity { get; set; }

        public required String Command { get; set; }

        public String? ReversaleDTO { get; set; }

        public required bool CollisionChecked { get; set; }

        public required Guid ScheduleId { get; set; }
    }
}
