using System.ComponentModel.DataAnnotations;

namespace AlpimiAPI.Entities.EAvailability.DTO
{
    public class CreateAvailabilityDTO
    {
        [LocalizedRequired]
        public required int? WeekDay { get; set; } //ex. 0 means Sunday

        [LocalizedRequired]
        public required int? Start { get; set; }

        [LocalizedRequired]
        public required int? End { get; set; }

        [LocalizedRequired]
        public required Guid? TeacherId { get; set; }
    }
}
