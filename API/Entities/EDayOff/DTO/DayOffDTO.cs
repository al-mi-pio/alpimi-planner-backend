namespace AlpimiAPI.Entities.EDayOff.DTO
{
    public class DayOffDTO
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }

        public required DateOnly From { get; set; }

        public required DateOnly To { get; set; }
    }
}
