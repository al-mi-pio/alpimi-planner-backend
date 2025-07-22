namespace AlpimiAPI.Entities.EUser.DTO
{
    public class CreateUserDTO
    {
        [LocalizedRequired]
        public required string? Login { get; set; }

        [LocalizedRequired]
        public required string? CustomURL { get; set; }

        [LocalizedRequired]
        public required string? Password { get; set; }
    }
}
