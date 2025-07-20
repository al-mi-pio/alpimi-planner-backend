namespace AlpimiAPI.Entities.EAuth.DTO
{
    public record LoginDTO
    {
        [LocalizedRequired]
        public required string? Login { get; set; }

        [LocalizedRequired]
        public required string? Password { get; set; }
    }
}
