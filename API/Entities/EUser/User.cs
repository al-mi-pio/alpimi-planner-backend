namespace AlpimiAPI.Entities.EUser
{
    public class User
    {
        public Guid Id { get; set; }

        public required string Login { get; set; }

        public string? CustomURL { get; set; } = null;
    }
}
