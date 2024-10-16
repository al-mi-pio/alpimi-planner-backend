namespace AlpimiAPI.Auth
{
    public class Auth
    {
        public Guid Id { get; set; }
        public string Password { get; set; }
        public Guid UserID { get; set; }
        public AlpimiAPI.User.User User { get; set; }
    }
}
