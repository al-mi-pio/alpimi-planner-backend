namespace AlpimiAPI.User
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Login { get; set; }
        public required string CustomURL { get; set; }
    }
}
