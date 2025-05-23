using System.ComponentModel.DataAnnotations;
using AlpimiAPI.Entities.ETeacher.DTO;

namespace AlpimiAPI.Entities.EAvailability.DTO
{
    public class AvailabilityDTO
    {
        public required Guid Id { get; set; }

        public required int? WeekDay { get; set; } //ex. 0 means Sunday

        public required int Start { get; set; }

        public required int End { get; set; }

        public required TeacherDTO Teacher { get; set; }
    }
}
