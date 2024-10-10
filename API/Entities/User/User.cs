namespace AlpimiAPI.User
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string CustomURL { get; set; }
    }
}
