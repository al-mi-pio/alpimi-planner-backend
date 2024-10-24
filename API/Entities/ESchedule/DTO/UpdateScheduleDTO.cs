using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.ESchedule.DTO
{
    public class UpdateScheduleDTO
    {
        public string? Name { get; set; }

        public int? SchoolHour { get; set; }
    }
}
