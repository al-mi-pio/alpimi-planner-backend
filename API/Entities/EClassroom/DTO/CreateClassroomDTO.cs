namespace AlpimiAPI.Entities.EClassroom.DTO
{
    public class CreateClassroomDTO
    {
        [LocalizedRequired]
        public required string? Name { get; set; }

        [LocalizedRequired]
        public required int? Capacity { get; set; }

        [LocalizedRequired]
        public required Guid? ScheduleId { get; set; }

        public IEnumerable<Guid>? ClassroomTypeIds { get; set; }
    }
}
