using AlpimiAPI.Entities.ETeacher;

namespace AlpimiAPI.Entities.EAvailability
{
    public class Availability
    {
        public Guid Id { get; set; }

        public required int WeekDay { get; set; } //ex. 0 means Sunday
        public required int Start { get; set; }

        public required int End { get; set; }

        public Guid TeacherId { get; set; }

        public required Teacher Teacher { get; set; }
    }
}
