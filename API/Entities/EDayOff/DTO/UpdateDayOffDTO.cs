namespace AlpimiAPI.Entities.EDayOff.DTO
{
    public class UpdateDayOffDTO
    {
        public string? Name { get; set; }

        public DateOnly? From { get; set; }

        public DateOnly? To { get; set; }
    }
}
