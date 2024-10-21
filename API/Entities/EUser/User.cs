namespace AlpimiAPI.Entities.EUser
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Login { get; set; }
        public required string CustomURL { get; set; }
    }
}
