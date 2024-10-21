namespace AlpimiAPI.Entities.EUser.DTO
{
    public class UpdateUserDTO
    {
        public required string? Login { get; set; }

        public required string? CustomURL { get; set; }
    }
}
