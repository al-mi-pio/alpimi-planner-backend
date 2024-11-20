using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.ESchedule.DTO
{
    public class CreateScheduleDTO
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public required int SchoolHour { get; set; }

        [Required]
        public required DateTime SchoolYearStart { get; set; }

        [Required]
        public required DateTime SchoolYearEnd { get; set; }
    }
}
