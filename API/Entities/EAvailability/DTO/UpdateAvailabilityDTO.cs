namespace AlpimiAPI.Entities.EAvailability.DTO
{
    public class UpdateAvailabilityDTO
    {
        public int? WeekDay { get; set; } //ex. 0 means Sunday

        public int? Start { get; set; }

        public int? End { get; set; }

        public Guid? TeacherId { get; set; }
    }
}
