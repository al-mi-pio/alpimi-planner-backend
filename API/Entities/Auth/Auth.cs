namespace AlpimiAPI.Entities.Auth
{
    public class Auth
    {
        public Guid Id { get; set; }
        public required string Password { get; set; }
        public Guid UserID { get; set; }
        public required string Salt { get; set; }
        public required AlpimiAPI.Entities.User.User User { get; set; }
    }
}
