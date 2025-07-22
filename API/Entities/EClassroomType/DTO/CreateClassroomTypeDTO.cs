namespace AlpimiAPI.Entities.EClassroomType.DTO
{
    public class CreateClassroomTypeDTO
    {
        [LocalizedRequired]
        public required string? Name { get; set; }

        [LocalizedRequired]
        public required Guid? ScheduleId { get; set; }
    }
}
