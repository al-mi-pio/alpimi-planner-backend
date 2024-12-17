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
        public required DateOnly SchoolYearStart { get; set; }

        [Required]
        public required DateOnly SchoolYearEnd { get; set; }

        [Required]
        public required string SchoolDays { get; set; }
    }
}
