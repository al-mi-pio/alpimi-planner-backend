using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.ETeacher.DTO
{
    public class CreateTeacherDTO
    {
        [LocalizedRequired]
        public required string? Name { get; set; }

        [LocalizedRequired]
        public required string? Surname { get; set; }

        [LocalizedRequired]
        public required Guid? ScheduleId { get; set; }
    }
}
